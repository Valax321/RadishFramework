using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Radish.Framework;
using Radish.Graphics;
using Radish.Services;

namespace Radish.UI;

[PublicAPI]
public sealed class ImGuiManager : 
    IDisposable, 
    IGameDraw, 
    IGameUpdate,
    IServiceConsumer
{
    // Needs to update FIRST and draw LAST
    public int DrawOrder => int.MaxValue;
    public int UpdateOrder => int.MinValue;

    private IntPtr _context;
    private IGraphicsDevice _graphicsDevice = null!;

    public ImGuiManager(GameServiceCollection services)
    {
        services.AddSingleton(this);
        
        _context = ImGui.CreateContext();
    }
    
    public void ResolveServices(IServiceProvider services)
    {
        _graphicsDevice = services.GetRequiredService<IGraphicsDevice>();
    }
    
    public void Update(TimeSpan deltaTime)
    {
        ImGui.NewFrame();
    }
    
    public void Draw(TimeSpan deltaTime)
    {
        ImGui.Render();
    }
    
    public void Dispose()
    {
        ImGui.DestroyContext(_context);
        _context = IntPtr.Zero;
    }
}
