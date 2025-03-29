using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Radish.Serialization.Xml.Serializers;

namespace Radish.Serialization.Xml;

public static class XmlSerializerRegistry
{
    internal static readonly Dictionary<Type, IXmlValueSerializer> TypeSerializers = [];
    
    [PublicAPI]
    public static void RegisterSerializer<TValue, TSerializer>() where TSerializer : XmlValueSerializer<TValue>, new()
    {
        if (!TypeSerializers.TryAdd(typeof(TValue), new TSerializer()))
        {
            throw new InvalidOperationException(
                $"Cannot register multiple XmlValueSerializers for {typeof(TValue).FullName}");
        }
    }
    
#pragma warning disable CA2255
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Init()
    {
        RegisterSerializer<byte, NumberSerializer<byte>>();
        RegisterSerializer<sbyte, NumberSerializer<sbyte>>();
        RegisterSerializer<ushort, NumberSerializer<ushort>>();
        RegisterSerializer<short, NumberSerializer<short>>();
        RegisterSerializer<uint, NumberSerializer<uint>>();
        RegisterSerializer<int, NumberSerializer<int>>();
        RegisterSerializer<ulong, NumberSerializer<ulong>>();
        RegisterSerializer<long, NumberSerializer<long>>();
        RegisterSerializer<float, NumberSerializer<float>>();
        RegisterSerializer<double, NumberSerializer<double>>();
        RegisterSerializer<Vector2, Vector2Serializer>();
        RegisterSerializer<Vector3, Vector3Serializer>();
        RegisterSerializer<Vector4, Vector4Serializer>();
        RegisterSerializer<Quaternion, QuaternionSerializer>();
        RegisterSerializer<string, StringSerializer>();
        RegisterSerializer<Guid, GuidSerializer>();
        RegisterSerializer<char, NumberSerializer<char>>();
        RegisterSerializer<bool, BoolSerializer>();
    }
}