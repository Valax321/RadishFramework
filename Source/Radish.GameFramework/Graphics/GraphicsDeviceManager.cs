using System.Diagnostics;
using System.Drawing;
using Radish.Framework;
using Radish.Platform;

namespace Radish.Graphics;

public class GraphicsDeviceManager : IDisposable, IGraphicsDevice
{
    private IPlatformRenderer Renderer { get; }
    private IntPtr _device;
    private readonly Window _window;

    private HashSet<IntPtr> _allocatedTextures = [];

    public bool EnableVSync
    {
        get => _vsyncEnabled;
        set
        {
            if (_device == IntPtr.Zero)
                return;

            _vsyncEnabled = Renderer.SetVsyncEnabled(_device, _window.Handle, value);
        }
    }

    private bool _vsyncEnabled;
    
    public GraphicsDeviceManager(Application app)
    {
        Renderer = app.Platform.GetRenderBackend();

        _window = app.Window;
        _device = Renderer.InitDeviceWithWindowHandle(_window.Handle);
        Renderer.SetVsyncEnabled(_device, _window.Handle, true);
    }

    internal void BeginFrame()
    {
        Renderer.BeginFrame(_device, _window.Handle);
    }

    internal void EndFrame()
    {
        Renderer.EndFrame(_device);
    }

    public IntPtr AcquireTexture2D(TextureFormat format, Size size, bool isLinear, bool hasMipmaps)
    {
        if (size.Width <= 0 || size.Height <= 0)
            throw new InvalidOperationException("Width and Height must be greater than 0");
        return Renderer.AcquireTexture(_device,
            new TextureCreationOptions(format, 
                TextureKind.Tex2D,
                TextureUsage.TextureSampler,
                !isLinear, hasMipmaps, 
                [(uint)size.Width, (uint)size.Height]));
    }

    public IntPtr AcquireRenderTexture2D(TextureFormat format, Size size, bool isLinear, bool hasMipmaps,
        AntialiasingLevel msaaLevel, TextureUsage usage)
    {
        if (size.Width <= 0 || size.Height <= 0)
            throw new InvalidOperationException("Width and Height must be greater than 0");
        return Renderer.AcquireTexture(_device,
            new TextureCreationOptions(format, TextureKind.Tex2D, usage, !isLinear, hasMipmaps,
                [(uint)size.Width, (uint)size.Height], msaaLevel));
    }

    public void ReleaseTexture(IntPtr handle)
    {
        _allocatedTextures.Remove(handle);
        Renderer.ReleaseTexture(_device, handle);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        Renderer.ReleaseDevice(_device, _window.Handle);
        _device = IntPtr.Zero;
    }
}