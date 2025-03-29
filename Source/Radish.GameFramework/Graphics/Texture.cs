namespace Radish.Graphics;

public abstract class Texture(Action<IntPtr> releaseAction) : IDisposable
{
    public TextureFormat Format { get; protected init; }
    public IntPtr ResourceHandle { get; protected set; }
    public bool IsLinear { get; protected set; }
    public bool HasMipmaps { get; protected set; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        releaseAction(ResourceHandle);
        ResourceHandle = IntPtr.Zero;
    }
}