namespace Radish.Filesystem.Pak;

public class PakFileSource(string pakPath, string absolutePath)
{
    public string PakPath { get; } = pakPath.ToLowerInvariant();
    public string AbsolutePath { get; } = absolutePath;
}