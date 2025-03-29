using JetBrains.Annotations;

namespace Radish;

[PublicAPI]
public abstract class ClassRegistry<T>
{
    private static readonly Dictionary<ClassId, Type> _typeRegistry = [];

    protected static IReadOnlyDictionary<ClassId, Type> TypeRegistry => _typeRegistry;

    protected static void RegisterClass<TClass>() where TClass : T
    {
        _typeRegistry.Add(ClassId.FromType<T>(), typeof(T));
    }
}