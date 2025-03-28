using JetBrains.Annotations;

namespace Radish.Framework;

[PublicAPI]
public abstract class GameComponent : 
    IGameInitialize, 
    IGameUpdate, 
    IDisposable
{
    public virtual int UpdateOrder => 0;
    
    public virtual void Initialize()
    {
        
    }
    
    public virtual void Update(TimeSpan deltaTime)
    {
        
    }
    
    protected virtual void Dispose(bool disposing)
    {}
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);   
    }
}