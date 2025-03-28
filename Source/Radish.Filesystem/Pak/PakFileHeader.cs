using System.Collections.Immutable;

namespace Radish.Filesystem.Pak;

public class PakFileHeader
{
    public const uint PakHeaderSignature = 1380991307;
    public const int PakVersion = 100;
    
    public uint Signature { get; private init; } = PakHeaderSignature;
    public int Version { get; private init; } = PakVersion;
    public int PartitionCount { get; set; } // Can be 0 (loaded from header file itself then)
    public int Revision { get; set; }

    public IReadOnlyDictionary<string, PakFileEntry> Entries { get; private set; } =
        new Dictionary<string, PakFileEntry>();

    public bool IsValid => Signature == PakHeaderSignature && Version == PakVersion;

    public static readonly PakFileHeader Invalid = new()
    {
        Signature = 0,
        Version = 0
    };

    public static PakFileHeader Read(BinaryReader reader, bool makeListImmutable = true)
    {
        var signature = reader.ReadUInt32();
        var version = reader.ReadInt32();
        var revision = reader.ReadInt32();
        var partitions = reader.ReadUInt16();

        var hdr = new PakFileHeader
        {
            Signature = signature,
            Version = version,
            PartitionCount = partitions,
            Revision = revision
        };

        if (!hdr.IsValid)
            return hdr;

        var entryCount = reader.ReadUInt32();
        var entries = new Dictionary<string, PakFileEntry>(StringComparer.InvariantCultureIgnoreCase);
        for (var i = 0; i < entryCount; ++i)
        {
            var e = new PakFileEntry();
            e.Path = reader.ReadString();
            e.Partition = reader.ReadUInt16();
            e.Offset = reader.ReadUInt32();
            e.Length = reader.ReadUInt32();
            e.Crc = reader.ReadUInt32();
            e.Flags = (PakFileEntry.EntryFlags)reader.ReadUInt32();
            
            entries.Add(e.Path, e);
        }

        if (makeListImmutable)
        {
            hdr.Entries = entries.ToImmutableDictionary();
        }
        else
        {
            hdr.Entries = entries;
        }

        return hdr;
    }

    public void WriteTo(BinaryWriter writer)
    {
        writer.Write(PakHeaderSignature);
        writer.Write(PakVersion);
        writer.Write(Revision);
        writer.Write((ushort)PartitionCount);
        
        writer.Write((uint)Entries.Count);
        foreach (var e in Entries)
        {
            writer.Write(e.Key);
            writer.Write((ushort)e.Value.Partition);
            writer.Write(e.Value.Offset);
            writer.Write(e.Value.Length);
            writer.Write(e.Value.Crc);
            writer.Write((uint)e.Value.Flags);
        }
    }
}