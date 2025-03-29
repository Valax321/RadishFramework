using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Radish.Serialization;

internal abstract class SerializerBase<TNodeType, TDocumentType>
{
    protected void Serialize([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)] 
        Type type, object value, Stream destination)
    {
        CreateDocument(type, out var doc, out var root);
        WriteClassValue(type, value, root);
        WriteToStream(doc, destination);
    }

    protected void WriteClassValue([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)] 
        Type type, object? value, TNodeType parent)
    {
        var members = new List<(MemberInfo, object?)>();
        
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
            {
                members.Add((field, field.GetValue(value)));
            }
        }

        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (prop.GetMethod == null || prop.GetCustomAttribute<NonSerializedAttribute>() != null)
                continue;
            
            members.Add((prop, prop.GetValue(value)));
        }

        foreach (var m in members.OrderBy(x => x.Item1.Name))
        {
            WriteNodeForValue(m.Item1.Name, m.Item1, m.Item2, parent);
        }
    }
    
    protected abstract void WriteNodeForValue(string propertyName, MemberInfo info, object? value, TNodeType parent);
    protected abstract void CreateDocument(Type rootObjectType, out TDocumentType outDocument, out TNodeType outRootNode);
    protected abstract void WriteToStream(TDocumentType document, Stream destination);
}