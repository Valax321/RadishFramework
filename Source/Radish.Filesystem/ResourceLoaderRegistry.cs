using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Radish.Filesystem;

public static class ResourceLoaderRegistry
{
    private class Entry
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public required Type LoaderType { get; init; }
        public required Type ResourceType { get; init; }
    }
    
    private static readonly Dictionary<ClassId, Entry> _typeRegistry = [];
    
    [PublicAPI]
    public static void Register<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TLoader, 
        TResource>() where TLoader : IResourceLoader<TResource>
    {
        _typeRegistry.Add(ClassId.FromType<TResource>(), new Entry
        {
            LoaderType = typeof(TLoader),
            ResourceType = typeof(TResource)
        });
    }
    
    internal static bool TryCreateLoader<TResource>(
        [NotNullWhen(true)] out IResourceLoader<TResource>? loader)
    {
        if (!_typeRegistry.TryGetValue(ClassId.FromType<TResource>(), out var e))
        {
            loader = null;
            return false;
        }
        
        Debug.Assert(e.LoaderType.IsAssignableTo(typeof(IResourceLoader<TResource>)));
        var l = Activator.CreateInstance(e.LoaderType);
        ArgumentNullException.ThrowIfNull(l);
        loader = (IResourceLoader<TResource>)l;
        return true;
    }
}