namespace Radish.Graphics;

[Flags]
public enum TextureUsage : uint
{
    None = 0,
    TextureSampler = 1 << 0,
    ColorTarget = 1 << 1,
    DepthTarget = 1 << 2,
}