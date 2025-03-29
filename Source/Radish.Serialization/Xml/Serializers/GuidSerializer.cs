using System.Globalization;
using System.Xml.Linq;

namespace Radish.Serialization.Xml.Serializers;

internal sealed class GuidSerializer : XmlValueSerializer<Guid>
{
    public override void Serialize(XElement parent, string name, Guid value)
    {
        var n = new XElement(name,
            new XAttribute("type", typeof(Guid).FullName!),
            new XAttribute("value", value.ToString("N", CultureInfo.InvariantCulture)));
        parent.Add(n);
    }

    public override Guid Deserialize(XElement parent, string name)
    {
        var n = parent.Element(name);
        var a = n?.Attribute("value");
        if (a != null && Guid.TryParse(a.Value, CultureInfo.InvariantCulture, out var guid))
        {
            return guid;
        }

        return Guid.Empty;
    }
}