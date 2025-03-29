using System.Globalization;
using System.Xml.Linq;

namespace Radish.Serialization.Xml.Serializers;

internal sealed class BoolSerializer : XmlValueSerializer<bool>
{
    public override void Serialize(XElement parent, string name, bool value)
    {
        var node = new XElement(name,
            new XAttribute("type", typeof(string).FullName!),
            new XAttribute("value", value.ToString(CultureInfo.InvariantCulture)));
        parent.Add(node);
    }

    public override bool Deserialize(XElement parent, string name)
    {
        var n = parent.Element(name);
        var a = n?.Attribute("value");
        if (a != null && bool.TryParse(a.Value, out var v))
            return v;
        return false;
    }
}