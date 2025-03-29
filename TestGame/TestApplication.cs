using Radish.Filesystem;
using Radish.Framework;
using Radish.Graphics;
using Radish.Logging;
using Radish.UI;

namespace Radish.TestGame;

public class TestApplication : Application
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<TestApplication>();
    
    public ResourceManager ResourceManager { get; }
    public ImGuiManager ImGuiManager { get; }
    public GraphicsDeviceManager GraphicsDeviceManager { get; }
    
    public TestApplication(in ApplicationOptions options) : base(in options)
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        ResourceManager = new ResourceManager(ServiceCollection,
            new ResourceManagerOptions(Platform.GetBasePath()));
        
        ImGuiManager = new ImGuiManager(ServiceCollection);
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        Logger.Info("Test game initialized");
    }
}