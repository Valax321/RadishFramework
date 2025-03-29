using Radish.Filesystem;
using Radish.Framework;
using Radish.Logging;
using Radish.UI;

namespace Radish.TestGame;

public class TestApplication : Application
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<TestApplication>();
    
    public ResourceManager ResourceManager { get; }
    public ImGuiManager ImGuiManager { get; }
    
    public TestApplication(in ApplicationOptions options) : base(in options)
    {
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