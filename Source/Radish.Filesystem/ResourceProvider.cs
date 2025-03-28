namespace Radish.Filesystem;

/// <summary>
/// Provides a source for loading resources, e.g. from the filesystem or pak files.
/// </summary>
public abstract class ResourceProvider(ResourceManager context)
{
    protected ResourceManager Context { get; } = context;
    public string BasePath => Context.BaseResourcesPath;
}