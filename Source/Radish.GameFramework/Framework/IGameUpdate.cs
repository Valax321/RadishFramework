namespace Radish.Framework;

public interface IGameUpdate
{
    int UpdateOrder { get; }
    
    void Update(TimeSpan deltaTime);
}