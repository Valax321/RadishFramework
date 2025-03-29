using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Radish.Graphics;
using Radish.Input;
using Radish.Logging;
using Radish.Platform;
using Radish.Services;

namespace Radish.Framework;

/// <summary>
/// Entry point for a RadishFramework game.
/// Should be subclassed by the game.
/// </summary>
[PublicAPI]
public abstract class Application : IDisposable
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<Application>();
    
    public IPlatformBackend Platform { get; }
    public GameServiceCollection ServiceCollection { get; }
    public Window Window { get; }
    
    public ServiceProvider? Services { get; private set; }
    
    [MemberNotNullWhen(true, nameof(Services))]
    public bool IsRunning { get; private set; }

    private List<IGameUpdate> _orderedUpdatables = [];
    private List<IGameDraw> _orderedDrawables = [];
    private double _lastUpdateTime;
    private IGraphicsDevice _graphicsDevice;

    protected Application(in ApplicationOptions options)
    {
        ServiceCollection = [];
        Platform = options.PlatformFactory();
        ServiceCollection.AddSingleton(Platform);

        Window = new Window(Platform, new Size(1280, 720));
        Window.Title = Assembly.GetEntryAssembly()?.GetName().Name ?? "Radish Game";
        
        Keyboard.SetPlatformBackend(Platform);
        Gamepad.SetPlatformBackend(Platform);
    }

    public void Run()
    {
        ResolveServices(ServiceCollection);
        Initialize();
        RunMainLoop();
        Quit();
    }

    private void ResolveServices(GameServiceCollection collection)
    {
        Services = collection.BuildServiceProvider();
        foreach (var c in Services.GetServices<IServiceConsumer>())
            c.ResolveServices(Services);

        _graphicsDevice = Services.GetRequiredService<IGraphicsDevice>();
        
        _orderedUpdatables.AddRange(Services.GetServices<IGameUpdate>()
            .OrderBy(x => x.UpdateOrder));
        _orderedDrawables.AddRange(Services.GetServices<IGameDraw>()
            .OrderBy(x => x.DrawOrder));
    }

    protected virtual void Initialize()
    {
        Debug.Assert(Services != null);
        
        foreach (var g in Services!.GetServices<IGameInitialize>())
            g.Initialize();
        
        Window.Show();
        IsRunning = true;
    }

    private void RunMainLoop()
    {
        while (IsRunning)
        {
            Platform.PumpEvents();
            var dt = CalculateDeltaTime();
            Update(dt);
            
            _graphicsDevice.BeginFrame();
            Draw(dt);
            _graphicsDevice.EndFrame();

            if (Platform.WantsToQuit)
                IsRunning = false;
        }
    }

    protected virtual void Update(TimeSpan deltaTime)
    {
        Debug.Assert(IsRunning);

        foreach (var g in _orderedUpdatables)
        {
            g.Update(deltaTime);
        }
    }

    protected virtual void Draw(TimeSpan deltaTime)
    {
        Debug.Assert(IsRunning);
        
        foreach (var g in _orderedDrawables)
        {
            g.Draw(deltaTime);
        }
    }

    protected virtual void Quit()
    {
        
    }
    
    protected virtual void Dispose(bool disposing)
    {}

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        Dispose(true);
        Services?.Dispose();
        Platform.Dispose(); // Should go last
    }

    private TimeSpan CalculateDeltaTime()
    {
        var now = Platform.GetPerformanceCounter();
        var scale = Platform.GetPerformanceCounterFrequency();
        var diff = now - _lastUpdateTime;
        _lastUpdateTime = now;
        return TimeSpan.FromSeconds(diff / scale);
    }
}
