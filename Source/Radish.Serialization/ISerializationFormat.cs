using System.Diagnostics.CodeAnalysis;

namespace Radish.Serialization;

internal interface ISerializationFormat
{
    static abstract void Serialize<[DynamicallyAccessedMembers(Serializer.RequiredMembers)] T>(T obj, Stream outputStream);
    static abstract T Deserialize<[DynamicallyAccessedMembers(Serializer.RequiredMembers)] T>(Stream inputStream);
}
