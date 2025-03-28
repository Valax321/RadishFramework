namespace Radish.Framework;

public interface IGameDraw
{
    int DrawOrder { get; }
    
    void Draw(TimeSpan deltaTime);
}