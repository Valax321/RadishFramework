using Radish.Platform;

namespace Radish.Framework;

public readonly record struct ApplicationOptions(Func<IPlatformBackend> PlatformFactory);
