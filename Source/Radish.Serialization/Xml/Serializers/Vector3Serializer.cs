using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace Radish.Serialization.Xml.Serializers;

internal sealed class Vector3Serializer : XmlValueSerializer<Vector3>
{
    public override void Serialize(XElement parent, string name, Vector3 value)
    {
        var node = new XElement(name, 
            new XAttribute("type", typeof(Vector3).FullName!),
            new XAttribute("x", value.X.ToString(NumberFormatInfo.InvariantInfo)),
            new XAttribute("y", value.Y.ToString(NumberFormatInfo.InvariantInfo)),
            new XAttribute("z", value.Z.ToString(NumberFormatInfo.InvariantInfo)));
        parent.Add(node);
    }

    public override Vector3 Deserialize(XElement parent, string name)
    {
        var node = parent.Element(name);
        if (node == null)
            return default;

        var x = node.Attribute("x");
        var y = node.Attribute("y");
        var z = node.Attribute("z");
        var v = new Vector3();
        if (x != null && float.TryParse(x.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var xv))
            v.X = xv;
        if (y != null && float.TryParse(y.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var yv))
            v.Y = yv;
        if (z != null && float.TryParse(z.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var zv))
            v.Z = zv;
        return v;
    }
}