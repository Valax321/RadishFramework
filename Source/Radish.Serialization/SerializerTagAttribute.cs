namespace Radish.Serialization;

/// <summary>
/// The tag represents the 4-letter code that is used in the extension for serialized representations.
/// </summary>
/// <param name="tag"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class SerializerTagAttribute(string tag) : Attribute
{
    public string Tag { get; } = tag.ToLower();
}
