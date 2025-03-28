using JetBrains.Annotations;

namespace Radish.Filesystem;

[PublicAPI]
public interface IResourceLoader<T>
{
    /// <summary>
    /// Loads a resource using the given context information.
    /// </summary>
    /// <param name="context">The loading context.</param>
    /// <returns>The loaded resource.</returns>
    T Load(in ResourceLoadContext context);

    /// <summary>
    /// Asynchronously loads a resource using the given context information.
    /// </summary>
    /// <param name="context">The loading context.</param>
    /// <param name="cancellationToken">Cancellation token for the load request.</param>
    /// <returns>Task for the load request.</returns>
    Task<T> LoadAsync(ResourceLoadContext context, CancellationToken cancellationToken) 
        => Task.Run(() => Load(context), cancellationToken);
}