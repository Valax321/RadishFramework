namespace Radish.Filesystem.Pak;

public class PakFileEntry
{
    [Flags]
    public enum EntryFlags : uint
    {
        None = 0,
    }
    
    public string Path { get; set; } = string.Empty;
    public int Partition { get; set; } = -1;
    public uint Length { get; set; }
    public uint Offset { get; set; }
    public EntryFlags Flags { get; set; }
    public uint Crc { get; set; }
}