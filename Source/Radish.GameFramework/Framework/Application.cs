using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Radish.Graphics;
using Radish.Logging;
using Radish.Platform;

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
    protected GameServiceCollection ServiceCollection { get; }
    public GraphicsDeviceManager GraphicsDeviceManager { get; }
    public Window Window { get; }
    
    public ServiceProvider? Services { get; private set; }
    
    [MemberNotNullWhen(true, nameof(Services))]
    public bool IsRunning { get; private set; }

    protected Application(in ApplicationOptions options)
    {
        ServiceCollection = [];
        Platform = options.PlatformFactory();
        ServiceCollection.AddSingleton(Platform);

        Window = new Window(Platform);
        Window.Title = Assembly.GetEntryAssembly()?.FullName ?? "Radish Game";

        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        ServiceCollection.AddSingleton(GraphicsDeviceManager);
    }

    public void Run()
    {
        Initialize();
        RunMainLoop();
        Quit();
    }

    protected virtual void Initialize()
    {
        Services = ServiceCollection.BuildServiceProvider();
        foreach (var g in Services.GetServices<IGameInitialize>())
        {
            g.Initialize();
        }
        
        IsRunning = true;
        Logger.Info("Application initialized");
    }

    private void RunMainLoop()
    {
        while (IsRunning)
        {
            Platform.PumpEvents();
            var dt = CalculateDeltaTime();
            Update(dt);

            if (Platform.WantsToQuit)
                IsRunning = false;
        }
    }

    protected virtual void Update(TimeSpan deltaTime)
    {
        Debug.Assert(IsRunning);

        foreach (var g in Services.GetServices<IGameUpdate>()
                     .OrderBy(x => x.UpdateOrder))
        {
            g.Update(deltaTime);
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
        return TimeSpan.FromSeconds(1 / 60.0);
    }
}
