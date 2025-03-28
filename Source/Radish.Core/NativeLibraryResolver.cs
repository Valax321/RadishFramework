using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Radish;

/// <summary>
/// Helper class that overrides DllImport resolution.
/// </summary>
[PublicAPI]
public static class NativeLibraryResolver
{
    public static List<string> SearchPaths { get; } = [];
    
    /// <summary>
    /// Installs the DllImport handler for a given assembly.
    /// </summary>
    /// <param name="assembly">The assembly to install the handler for.</param>
    public static void InitializeForAssembly(Assembly assembly)
    {
        NativeLibrary.SetDllImportResolver(assembly, ResolveLibrary);
    }

    private static IntPtr ResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        return IntPtr.Zero;
    }
}