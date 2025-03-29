namespace Radish.Graphics;

public readonly record struct TextureCreationOptions(
    TextureFormat Format,
    TextureKind Kind,
    TextureUsage Usage,
    bool IsSRGB,
    bool Mipmaps,
    uint[] Dimensions,
    AntialiasingLevel MSAALevel = AntialiasingLevel.None
)
{
    public int MipmapLevels
    {
        get
        {
            if (!Mipmaps)
                return 1;
            throw new NotImplementedException();
        }
    }
}