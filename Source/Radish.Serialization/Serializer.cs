using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Radish.Serialization.Xml;

namespace Radish.Serialization;

[PublicAPI]
public static class Serializer
{
    public const DynamicallyAccessedMemberTypes RequiredMembers = DynamicallyAccessedMemberTypes.PublicFields |
                                                                   DynamicallyAccessedMemberTypes.PublicProperties |
                                                                   DynamicallyAccessedMemberTypes.NonPublicFields |
                                                                   DynamicallyAccessedMemberTypes.NonPublicProperties |
                                                                   DynamicallyAccessedMemberTypes
                                                                       .PublicParameterlessConstructor |
                                                                   DynamicallyAccessedMemberTypes.PublicNestedTypes |
                                                                   DynamicallyAccessedMemberTypes.NonPublicNestedTypes;

    public static void Serialize<
            [DynamicallyAccessedMembers(RequiredMembers)]
            T>
        (T obj, Format format, Stream outputStream)
    {
        switch (format)
        {
            case Format.Xml:
                XmlSerializationFormat.Serialize(obj, outputStream);
                break;
            case Format.Binary:
            //BinarySerializationFormat.Serialize(obj, outputStream);
            case Format.Yaml:
            case Format.Json:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static T Deserialize<
            [DynamicallyAccessedMembers(RequiredMembers)]
            T>
        (Format format, Stream inputStream)
    {
        switch (format)
        {
            case Format.Xml:
                return XmlSerializationFormat.Deserialize<T>(inputStream);
            case Format.Binary:
            //return BinarySerializationFormat.Deserialize<T>(inputStream);
            case Format.Yaml:
            case Format.Json:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}