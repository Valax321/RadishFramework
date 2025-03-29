using System.Diagnostics.CodeAnalysis;

namespace Radish.Graphics;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum TextureFormat
{
    RGBA8,
    BGRA8,
    RGBAHalf,
    RGBAFloat,
    DXT1,
    DXT3,
    DXT5,
    DXTnm,
    BC4,
    BC6H,
    BC7,
}