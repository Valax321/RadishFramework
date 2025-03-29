using System.Drawing;
using JetBrains.Annotations;

namespace Radish.Graphics;

[PublicAPI]
public sealed class RenderTexture2D : Texture
{
    public Size Size { get; }
    public TextureUsage Usage { get; }
    public AntialiasingLevel MSAALevel { get; }
    
    public RenderTexture2D(IGraphicsDevice device, Size size, TextureFormat format, bool isLinear, bool hasMips, TextureUsage usage, AntialiasingLevel msaaLevel = AntialiasingLevel.None) 
        : base(device.ReleaseTexture)
    {
        Size = size;
        Format = format;
        Usage = usage;
        MSAALevel = msaaLevel;
        IsLinear = isLinear;
        HasMipmaps = hasMips;
        ResourceHandle = device.AcquireRenderTexture2D(format, size, isLinear, hasMips, msaaLevel, usage);
    }
}