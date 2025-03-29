using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Radish.Framework;
using Radish.Logging;
using Radish.Platform;

namespace Radish.Graphics;

public class GraphicsDeviceManager : IDisposable, IGraphicsDevice
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<GraphicsDeviceManager>();
    
    private delegate void InternalResetDelegate(IntPtr window, Size size, uint display);
    private static InternalResetDelegate? _onResetCalledByBackend;
    
    private IPlatformRenderer Renderer { get; }
    private IntPtr _device;
    private readonly Window _window;
    private readonly IPlatformBackend _platform;

    private HashSet<IntPtr> _allocatedTextures = [];
    private IGraphicsDevice.DeviceResetDelegate? _onDeviceReset;

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

    public Size BackbufferSize { get; private set; }

    public float DisplayScale => _platform.GetWindowDisplayScale(_window.Handle);
    public uint DisplayIndex { get; private set; }

    private bool _vsyncEnabled;
    
    public GraphicsDeviceManager(Application app)
    {
        app.ServiceCollection.AddSingleton<IGraphicsDevice>(this);
        
        Renderer = app.Platform.GetRenderBackend();
        _onResetCalledByBackend += OnReset;

        _platform = app.Platform;

        _window = app.Window;
        _device = Renderer.InitDeviceWithWindowHandle(_window.Handle, out var s, out var d);
        BackbufferSize = s;
        DisplayIndex = d;
        _vsyncEnabled = Renderer.SetVsyncEnabled(_device, _window.Handle, true);
    }

    private void OnReset(IntPtr window, Size size, uint display)
    {
        if (window == _window.Handle)
        {
            Logger.Debug("Device reset: {0}, display {1}", size.ToString(), display);
            BackbufferSize = size;
            DisplayIndex = display;
            ResetCallback?.Invoke();
            
            // Since we might have moved to a new window, swapchain details may have changed
            _vsyncEnabled = Renderer.SetVsyncEnabled(_device, _window.Handle, EnableVSync);
        }
    }

    public void BeginFrame()
    {
        Renderer.BeginFrame(_device, _window.Handle);
    }

    public void EndFrame()
    {
        Renderer.EndFrame(_device);
    }

    public IGraphicsDevice.DeviceResetDelegate? ResetCallback { get; set; }

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
        _onResetCalledByBackend -= Reset;
    }
    
    internal static void Reset(IntPtr window, Size newBackbufferSize, uint newDisplayIndex)
    {
        _onResetCalledByBackend?.Invoke(window, newBackbufferSize, newDisplayIndex);
    }
}