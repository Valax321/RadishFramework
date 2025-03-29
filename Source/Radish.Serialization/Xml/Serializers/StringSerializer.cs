using System.Xml.Linq;

namespace Radish.Serialization.Xml.Serializers;

internal sealed class StringSerializer : XmlValueSerializer<string>
{
    public override void Serialize(XElement parent, string name, string? value)
    {
        var node = new XElement(name,
            new XAttribute("type", typeof(string).FullName!),
            value ?? string.Empty);
        parent.Add(node);
    }

    public override string Deserialize(XElement parent, string name)
    {
        var n = parent.Element(name);
        return n == null ? string.Empty : n.Value;
    }
}