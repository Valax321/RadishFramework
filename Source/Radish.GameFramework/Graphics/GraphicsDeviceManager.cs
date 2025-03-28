using Radish.Framework;
using Radish.Platform;

namespace Radish.Graphics;

public class GraphicsDeviceManager : IDisposable
{
    private IPlatformRenderer Renderer { get; }
    
    public GraphicsDeviceManager(Application app)
    {
        Renderer = app.Platform.GetRenderBackend();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}