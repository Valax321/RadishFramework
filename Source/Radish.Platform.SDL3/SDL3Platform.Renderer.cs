namespace Radish.Platform;

public partial class SDL3Platform : IPlatformRenderer
{
    public IPlatformRenderer GetRenderBackend() => this;
}