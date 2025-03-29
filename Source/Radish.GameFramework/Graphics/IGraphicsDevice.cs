using System.Drawing;

namespace Radish.Graphics;

public interface IGraphicsDevice
{
    IntPtr AcquireTexture2D(TextureFormat format, Size size, bool isLinear, bool hasMipmaps);

    IntPtr AcquireRenderTexture2D(TextureFormat format, Size size, bool isLinear, bool hasMipmaps,
        AntialiasingLevel msaaLevel, TextureUsage usage);

    void ReleaseTexture(IntPtr handle);
}