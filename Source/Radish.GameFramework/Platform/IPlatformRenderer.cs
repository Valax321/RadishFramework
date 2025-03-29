using System.Drawing;
using Radish.Graphics;

namespace Radish.Platform;

public interface IPlatformRenderer
{
    public IntPtr InitDeviceWithWindowHandle(IntPtr window, out Size pixelSize, out uint displayIndex);
    public void ReleaseDevice(IntPtr renderer, IntPtr window);
    public bool SetVsyncEnabled(IntPtr renderer, IntPtr window, bool enabled);

    public void BeginFrame(IntPtr renderer, IntPtr window);
    public void EndFrame(IntPtr renderer);

    public IntPtr AcquireTexture(IntPtr renderer, in TextureCreationOptions options);
    public void ReleaseTexture(IntPtr renderer, IntPtr handle);
}