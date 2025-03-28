using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Radish.Filesystem;

public readonly record struct ResourceManagerOptions(
    string BaseResourcesPath, 
    string? SourceContentPath = null
);

/// <summary>
/// Game component that manages the loading of files from game data.
/// </summary>
[PublicAPI]
public class ResourceManager
{
    /// <summary>
    /// The path where all resource data sources are located.
    /// </summary>
    public string BaseResourcesPath { get; }
    
    /// <summary>
    /// The path where source files are located, in case they can be re-compiled automatically.
    /// </summary>
    public string? SourceContentPath { get; }

    /// <summary>
    /// List of resource locations that have been mounted.
    /// </summary>
    public IReadOnlyList<ResourceProvider> ResourceProviders => _resourceProviders;
    
    private readonly List<ResourceProvider> _resourceProviders = [];
    
    public ResourceManager(IServiceCollection services, ResourceManagerOptions options)
    {
        services.AddSingleton(this);
        
        BaseResourcesPath = options.BaseResourcesPath;
        SourceContentPath = options.SourceContentPath;
    }
}
