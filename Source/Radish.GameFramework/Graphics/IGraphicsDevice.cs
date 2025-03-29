using System.Drawing;

namespace Radish.Graphics;

public interface IGraphicsDevice
{
    public delegate void DeviceResetDelegate();
    
    public event DeviceResetDelegate OnReset
    {
        add => ResetCallback += value;
        remove
        {
            if (ResetCallback != null)
                ResetCallback -= value;
        }
    }
    
    public Size BackbufferSize { get; }
    public float DisplayScale { get; }
    
    protected DeviceResetDelegate? ResetCallback { get; set; }

    void BeginFrame();
    void EndFrame();
    
    IntPtr AcquireTexture2D(TextureFormat format, Size size, bool isLinear, bool hasMipmaps);

    IntPtr AcquireRenderTexture2D(TextureFormat format, Size size, bool isLinear, bool hasMipmaps,
        AntialiasingLevel msaaLevel, TextureUsage usage);

    void ReleaseTexture(IntPtr handle);
}