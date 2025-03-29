using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using JetBrains.Annotations;
using Radish.Logging;
using Radish.Text;
using static SDL3.SDL;

namespace Radish.Platform;

public sealed partial class SDL3Platform : IPlatformBackend
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<SDL3Platform>();

    static SDL3Platform()
    {
        NativeLibraryResolver.InitializeForAssembly(Assembly.GetExecutingAssembly());
    }
    
    [PublicAPI]
    public static SDL3Platform Create() => new();
    
    public string Name => "SDL3";
    public bool WantsToQuit { get; private set; }

    private SDL3Platform()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly is not null)
        {
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            var version = assembly.GetCustomAttribute<AssemblyVersionAttribute>();
            var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            var name = assembly.GetName();

            var identifier = $"com.{company?.Company.ToLower()}.{name.Name?.ToLower()}";

            SDL_SetAppMetadata(
                product?.Product ?? string.Empty, 
                version?.Version ?? string.Empty, 
                identifier);
            SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING, "game");

            if (company is not null)
                SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING, company.Company);
            if (copyright is not null)
                SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_COPYRIGHT_STRING, copyright.Copyright);
        }
        
        var success = SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_GAMEPAD | SDL_InitFlags.SDL_INIT_HAPTIC |
                 SDL_InitFlags.SDL_INIT_SENSOR);
        if (!success)
            throw new PlatformException($"SDL_Init failed: {SDL_GetError()}");

        var v = SDL_GetVersion();
        Logger.Info("SDL version: {0}.{1}.{2}", v / 1000000, (v / 1000) % 1000, (v % 1000));
    }

    public void Dispose()
    {
        SDL_Quit();
    }
    
    public void PumpEvents()
    {
        WantsToQuit = false;
        while (SDL_PollEvent(out var ev))
        {
            ProcessEvent(ev);
        }
    }

    private void ProcessEvent(in SDL_Event ev)
    {
        switch ((SDL_EventType)ev.type)
        {
            case SDL_EventType.SDL_EVENT_QUIT:
                WantsToQuit = true;
                break;
            case SDL_EventType.SDL_EVENT_RENDER_TARGETS_RESET:
            case SDL_EventType.SDL_EVENT_RENDER_DEVICE_RESET:
                OnGpuDeviceReset(in ev);
                break;
            case SDL_EventType.SDL_EVENT_RENDER_DEVICE_LOST:
                OnGpuDeviceLost();
                break;
        }
    }
    
    public IntPtr CreateWindow(Size size)
    {
        const SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_HIDDEN 
                                      | SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;
        var wnd = SDL_CreateWindow("Window", size.Width, size.Height, flags);
        if (wnd == IntPtr.Zero)
        {
            throw new PlatformException($"Failed to create window: {SDL_GetError()}");
        }

        return wnd;
    }

    public void DestroyWindow(IntPtr window)
    {
        SDL_DestroyWindow(window);
    }

    public void SetWindowTitle(IntPtr window, string title)
    {
        Debug.Assert(window != IntPtr.Zero);
        SDL_SetWindowTitle(window, title);
    }

    public string GetWindowTitle(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return string.Empty;
        return SDL_GetWindowTitle(window);
    }

    public void SetWindowSize(IntPtr window, Size size)
    {
        Debug.Assert(window != IntPtr.Zero);
        SDL_SetWindowSize(window, size.Width, size.Height);
    }

    public Size GetWindowSize(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return Size.Empty;
        if (!SDL_GetWindowSize(window, out var w, out var h))
            return Size.Empty;
        return new Size(w, h);
    }

    public void ShowWindow(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return;

        SDL_ShowWindow(window);
    }

    public void HideWindow(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return;

        SDL_HideWindow(window);
    }

    public string GetBasePath()
    {
        return SDL_GetBasePath();
    }

    public string GetWritePath()
    {
        var creator = SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING);
        var name = SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_NAME_STRING);
        return SDL_GetPrefPath(creator.ReplaceAll(Path.GetInvalidPathChars(), '_'),
            name.ReplaceAll(Path.GetInvalidPathChars(), '_'));
    }

    public double GetPerformanceCounter()
    {
        return SDL_GetPerformanceCounter();
    }

    public double GetPerformanceCounterFrequency()
    {
        return SDL_GetPerformanceFrequency();
    }
}
