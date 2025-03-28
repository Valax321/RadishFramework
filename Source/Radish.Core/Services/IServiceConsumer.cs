using JetBrains.Annotations;

namespace Radish.Services;

/// <summary>
/// Marks that a class wishes to resolve some external services when the owning context is ready.
/// </summary>
[PublicAPI]
public interface IServiceConsumer
{
    /// <summary>
    /// Method for resolving required services.
    /// </summary>
    /// <param name="services">Service provider for the owning context.</param>
    void ResolveServices(IServiceProvider services);
}
