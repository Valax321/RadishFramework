using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace Radish.Serialization.Xml.Serializers;

internal sealed class QuaternionSerializer : XmlValueSerializer<Quaternion>
{
    public override void Serialize(XElement parent, string name, Quaternion value)
    {
        var node = new XElement(name, 
            new XAttribute("type", typeof(Quaternion).FullName!),
            new XAttribute("x", value.X.ToString(NumberFormatInfo.InvariantInfo)),
            new XAttribute("y", value.Y.ToString(NumberFormatInfo.InvariantInfo)),
            new XAttribute("z", value.Z.ToString(NumberFormatInfo.InvariantInfo)),
            new XAttribute("w", value.W.ToString(NumberFormatInfo.InvariantInfo)));
        parent.Add(node);
    }

    public override Quaternion Deserialize(XElement parent, string name)
    {
        var node = parent.Element(name);
        if (node == null)
            return default;

        var x = node.Attribute("x");
        var y = node.Attribute("y");
        var z = node.Attribute("z");
        var w = node.Attribute("w");
        var v = new Quaternion();
        if (x != null && float.TryParse(x.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var xv))
            v.X = xv;
        if (y != null && float.TryParse(y.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var yv))
            v.Y = yv;
        if (z != null && float.TryParse(z.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var zv))
            v.Z = zv;
        if (w != null && float.TryParse(w.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var vw))
            v.W = vw;
        return v;
    }
}