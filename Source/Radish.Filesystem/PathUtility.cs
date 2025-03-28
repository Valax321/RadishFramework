using JetBrains.Annotations;

namespace Radish.Filesystem;

/// <summary>
/// Wrapper methods to ensure <see cref="Path"/> APIs work across all platforms.
/// </summary>
[PublicAPI]
public static class PathUtility
{
    /// <summary>
    /// Should return false if this platform's .NET implementation of <see cref="Path"/>
    /// does not actually handle the platform file paths properly. This may be the case on
    /// e.g. Nintendo Switch but isn't the case for officially supported .NET platforms.
    /// </summary>
    private static bool IsSafeToUseBuiltinFunctions => 
        OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux();

    public static string Combine(params ReadOnlySpan<string> paths)
    {
        if (IsSafeToUseBuiltinFunctions)
            return Path.Combine(paths);
        throw new NotImplementedException();
    }

    public static string GetDirectoryName(string path)
    {
        if (IsSafeToUseBuiltinFunctions)
            return Path.GetDirectoryName(path) ?? string.Empty;
        throw new NotImplementedException();
    }

    public static string GetFileNameWithoutExtension(string path)
    {
        if (IsSafeToUseBuiltinFunctions)
            return Path.GetFileNameWithoutExtension(path);
        throw new NotImplementedException();
    }
}