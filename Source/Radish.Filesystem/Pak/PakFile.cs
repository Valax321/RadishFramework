using System.Collections;
using System.Diagnostics;
using System.IO.Hashing;
using JetBrains.Annotations;
using Radish.Filesystem.Pak.Substreams;
using Radish.Logging;
using Radish.Text;

namespace Radish.Filesystem.Pak;

/// <summary>
/// Advanced settings for the pak building process.
/// </summary>
/// <param name="PakFileAlignment">Alignment (in bytes) for files within the pak. Useful for ensuring files align to update/patch delta packet sizes.</param>
/// <param name="TargetPartitionSize">The target size (in bytes) for each part in a multipart pak file.
/// Note that parts can be somewhat larger than this in practice if large files are packed at the end of the part.</param>
public record struct PakBuildOptions(
    int PakFileAlignment = 4096, 
    long TargetPartitionSize = 209715200
);

[PublicAPI]
public sealed class PakFile : IReadOnlyCollection<string>
{
    public const string FileExtension = "rpk";
    
    private static readonly ILogger Logger = LogManager.GetLoggerForType<PakFile>();
    
    public PakFileHeader Header { get; private init; } = PakFileHeader.Invalid;
    public required string PakDirectory { get; init; }
    public required string Name { get; init; }

    public static PakFile Open(string path, bool isOpenForBuild = false)
    {
        using var fs = File.OpenRead(path);
        using var reader = new BinaryReader(fs);
        var pak = new PakFile
        {
            Header = PakFileHeader.Read(reader, !isOpenForBuild),
            PakDirectory = PathUtility.GetDirectoryName(path),
            Name = PathUtility.GetFileNameWithoutExtension(path).TrimEnd("_dir")
        };

        return pak;
    }

    public bool HasFile(string path)
    {
        return Header.Entries.ContainsKey(path);
    }

    public Stream? OpenRead(string path)
    {
        if (!Header.Entries.TryGetValue(path, out var e))
            return null;

        var f = File.OpenRead(MakePartitionPath(PakDirectory, Name, e.Partition, Header.PartitionCount >= 0));
        return f.Substream(e.Offset, e.Length);
    }

    /// <summary>
    /// Builds or updates a pak file on disk with the given files.
    /// </summary>
    /// <param name="name">The base name of the pak file.</param>
    /// <param name="path">The directory to create the pak file in.</param>
    /// <param name="multiPak">True to generate a multipart pak, false to generate a single file (header-only) pak.</param>
    /// <param name="paths">List of files to add to the pak.</param>
    /// <param name="options">Advanced options for the pak building process.</param>
    public static void BuildPakFile(string name, string path, bool multiPak, IEnumerable<PakFileSource> paths, PakBuildOptions options = new())
    {
        // TODO: this code is pretty fragile, might be worth cleaning up at some point
        
        var headerPath = Path.Combine(path, multiPak ? $"{name}_dir.{FileExtension}" : $"{name}.{FileExtension}");
        Logger.Info("Packing {0}", headerPath);
        PakFile f;
        var isUpdating = false;
        if (File.Exists(headerPath))
        {
            Logger.Info("Updating existing pak contents");
            f = Open(headerPath, true);
            f.Header.PartitionCount++;
            isUpdating = true;
        }
        else
        {
            f = new PakFile
            {
                Header = new PakFileHeader(),
                PakDirectory = string.Empty,
                Name = name
            };
        }
        
        if (!f.Header.IsValid)
            throw new PakFileException("Pak header invalid");

        f.Header.Revision++;

        var numUpdatedEntries = 0;
        foreach (var file in paths)
        {
            if (!f.Header.Entries.TryGetValue(file.PakPath, out var e))
            {
                Debug.Assert(f.Header.Entries is IDictionary<string, PakFileEntry>);
                var d = (IDictionary<string, PakFileEntry>)f.Header.Entries;
                e = new PakFileEntry
                {
                    Path = file.PakPath,
                    Partition = -1
                };
                d.Add(file.PakPath, e);
            }

            var fileInfo = new FileInfo(file.AbsolutePath);
            
            var crc = Crc32.HashToUInt32(File.ReadAllBytes(file.AbsolutePath));
            if (e.Partition >= 0)
            {
                if (crc == e.Crc)
                {
                    Logger.Info("Skipping {0} due to crc match", file.PakPath);
                    continue; // This file is the same as it was before, no need to update
                }
            }
            
            Logger.Info("Packing {0} to partition {1}", file.PakPath, f.Header.PartitionCount);

            using var currentPartitionStream =
                File.OpenWrite(MakePartitionPath(path, name, f.Header.PartitionCount, multiPak));
            currentPartitionStream.Seek(0, SeekOrigin.End);
            
            // Aligns this file to the correct boundary
            var position = currentPartitionStream.Position;
            if (position > 0)
            {
                var pakRemainder = position % options.PakFileAlignment;
                currentPartitionStream.Seek(pakRemainder, SeekOrigin.Current);
            }

            e.Partition = f.Header.PartitionCount;
            e.Crc = Crc32.HashToUInt32(File.ReadAllBytes(file.AbsolutePath));
            e.Offset = (uint)currentPartitionStream.Position;
            e.Length = (uint)fileInfo.Length;

            using var ff = File.OpenRead(file.AbsolutePath);
            ff.CopyTo(currentPartitionStream);

            if (currentPartitionStream.Position > options.TargetPartitionSize)
                f.Header.PartitionCount++;
            numUpdatedEntries++;
        }
        
        // Revert incremented partition count if we wrote nothing and are updating an existing pak
        if (numUpdatedEntries == 0 && isUpdating)
            f.Header.PartitionCount--;
        
        {
            using var hdrFile = File.OpenWrite(headerPath);
            hdrFile.SetLength(0);
            using var writer = new BinaryWriter(hdrFile);
            f.Header.WriteTo(writer);
        }
    }

    private static string MakePartitionPath(string path, string name, int partition, bool multiPak)
    {
        return Path.Combine(path, multiPak ? $"{name}_{MakePakPartitionFileNumber(partition)}.{FileExtension}" : $"{name}.{FileExtension}");
    }

    private static string MakePakPartitionFileNumber(int partition)
    {
        return partition.ToString("000");
    }

    public IEnumerator<string> GetEnumerator()
    {
        return Header.Entries.Select(f => f.Value.Path).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => Header.Entries.Count;
}