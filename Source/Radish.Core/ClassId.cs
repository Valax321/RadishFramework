using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Hashing;
using System.Text;
using System.Text.Unicode;
using JetBrains.Annotations;

namespace Radish;

/// <summary>
/// Numerical value representing a .NET class.
/// Intended to be used with <see cref="ClassRegistry{T}"/>.
/// </summary>
/// <param name="value">Serialized value of a class ID.</param>
[DebuggerDisplay("Value = {Value}")]
public readonly struct ClassId(uint value) : 
    IEquatable<ClassId>,
    ISpanParsable<ClassId>,
    IUtf8SpanParsable<ClassId>,
    ISpanFormattable,
    IUtf8SpanFormattable,
    IComparable<ClassId>
{
    public uint Value { get; } = value;

    /// <summary>
    /// Creates a class ID from the given type.
    /// </summary>
    /// <typeparam name="T">Type to create the ID from.</typeparam>
    /// <returns>ClassId for the given type.</returns>
    [PublicAPI]
    public static ClassId FromType<T>() => FromType(typeof(T));

    /// <summary>
    /// Creates a class ID from the given type.
    /// </summary>
    /// <seealso cref="FromType{T}"/>
    /// <param name="type">Type to create the ID from.</param>
    /// <returns>ClassId for the given type.</returns>
    [PublicAPI]
    public static unsafe ClassId FromType(Type type)
    {
        var typeName = type.FullName;
        ArgumentNullException.ThrowIfNull(typeName);
        
        // Class names are never particularly long so a stackalloc should be safe
        var length = Encoding.UTF8.GetByteCount(typeName);
        var data = stackalloc byte[length];
        Encoding.UTF8.GetBytes(typeName, new Span<byte>(data, length));
        return new ClassId(Crc32.HashToUInt32(new ReadOnlySpan<byte>(data, length)));
    }

    public bool Equals(ClassId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is ClassId other && Equals(other);
    }
    
    public int CompareTo(ClassId other)
    {
        return Value.CompareTo(other.Value);
    }

    public override int GetHashCode()
    {
        return (int)Value;
    }

    public static bool operator ==(ClassId left, ClassId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ClassId left, ClassId right)
    {
        return !(left == right);
    }

    public static ClassId Parse(string s, IFormatProvider? provider)
    {
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ClassId result)
    {
        if (s is not null) 
            return TryParse(s.AsSpan(), provider, out result);
        
        result = default;
        return false;
    }

    public static ClassId Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        var id = uint.Parse(s, NumberStyles.HexNumber | NumberStyles.Integer, provider);
        return new ClassId(id);
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ClassId result)
    {
        if (!uint.TryParse(s, NumberStyles.HexNumber | NumberStyles.Integer, provider, out var id))
        {
            result = new ClassId(id);
        }

        result = default;
        return false;
    }

    public static ClassId Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
    {
        var id = uint.Parse(utf8Text, NumberStyles.HexNumber | NumberStyles.Integer, provider);
        return new ClassId(id);
    }

    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out ClassId result)
    {
        if (!uint.TryParse(utf8Text, NumberStyles.HexNumber | NumberStyles.Integer, provider, out var id))
        {
            result = new ClassId(id);
        }

        result = default;
        return false;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"{nameof(Value)}: {Value}";
        return formattable.ToString(formatProvider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        return destination.TryWrite(provider, $"{nameof(Value)}: {Value}", out charsWritten);
    }

    public bool TryFormat(Span<byte> destination, out int bytesWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        return Utf8.TryWrite(destination, provider, $"{nameof(Value)}: {Value}", out bytesWritten);
    }

    public override string ToString()
    {
        return $"{nameof(Value)}: {Value}";
    }
}