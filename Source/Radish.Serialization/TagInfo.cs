using System.Reflection;
using JetBrains.Annotations;

namespace Radish.Serialization;

[PublicAPI]
public static class TagInfo
{
    public static string GetExtensionFromTagInfo<T>(Format format)
        => GetExtensionFromTagInfo(typeof(T), format);
    
    public static string GetExtensionFromTagInfo(Type t, Format format)
    {
        var tagAttr = t.GetCustomAttribute<SerializerTagAttribute>();
        if (tagAttr == null)
            throw new InvalidOperationException("Type must have a SerializerTagAttribute");

        return format switch
        {
            Format.Xml => $"{tagAttr.Tag}.xml",
            Format.Binary => $"{tagAttr.Tag}",
            Format.Yaml => $"{tagAttr.Tag}.yaml",
            Format.Json => $"{tagAttr.Tag}.json",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}