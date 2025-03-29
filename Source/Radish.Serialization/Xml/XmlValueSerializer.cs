using System.Diagnostics;
using System.Xml.Linq;

namespace Radish.Serialization.Xml;

public abstract class XmlValueSerializer<TValue> : IXmlValueSerializer
{
    public abstract void Serialize(XElement parent, string name, TValue? value);
    public abstract TValue? Deserialize(XElement parent, string name);
    
    public void SerializeInternal(XElement parent, string name, object? value)
    {
        Debug.Assert(value is null || value is TValue);
        Serialize(parent, name, value is null ? default : (TValue)value);
    }

    public object? DeserializeInternal(XElement parent, string name)
    {
        return Deserialize(parent, name);
    }
}