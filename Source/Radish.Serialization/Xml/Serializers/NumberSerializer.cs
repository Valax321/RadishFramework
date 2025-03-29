using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Radish.Serialization.Xml.Serializers;

[PublicAPI]
public class NumberSerializer<T> : XmlValueSerializer<T> where T : INumber<T>
{
    private const string AttributeName = "value";
    
    public override void Serialize(XElement parent, string name, T? value)
    {
        ArgumentNullException.ThrowIfNull(typeof(T).FullName);
        var node = new XElement(name, 
            new XAttribute("type", typeof(T).FullName!),
            new XAttribute(AttributeName, value?.ToString(null, NumberFormatInfo.InvariantInfo) ?? "null"));
        parent.Add(node);
    }

    public override T? Deserialize(XElement parent, string name)
    {
        ArgumentNullException.ThrowIfNull(typeof(T).FullName);
        var node = parent.Element(name);
        if (node == null)
            return default;

        var attr = node.Attribute(AttributeName);
        if (attr == null)
            throw new SerializationException("NumberSerializer expects a value attribute on its element");
        if (attr.Value == "null")
            return default;

        if (!T.TryParse(attr.Value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var v))
        {
            throw new SerializationException($"Failed to parse Value attribute as {typeof(T).FullName}");
        }

        return v;
    }
}