using System.Xml.Linq;

namespace Radish.Serialization.Xml;

internal interface IXmlValueSerializer
{
    internal void SerializeInternal(XElement parent, string name, object? value);
    internal object? DeserializeInternal(XElement parent, string name);
}