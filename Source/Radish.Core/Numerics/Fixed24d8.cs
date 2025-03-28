using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using JetBrains.Annotations;

namespace Radish.Numerics;

/// <summary>
/// 24.8 fixed point number
/// </summary>
[PublicAPI]
[DebuggerDisplay("{ToFloat(),nq}")]
public readonly struct Fixed24d8 : 
    ISignedNumber<Fixed24d8>
{
    public static Fixed24d8 MaxValue { get; } = new(int.MaxValue);
    public static Fixed24d8 MinValue { get; } = new(int.MinValue);
    
    public override bool Equals(object? obj)
    {
        return obj is Fixed24d8 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value;
    }

    public const int Bits = 8;
    public const int MaskBits = 0xff;
    private readonly int _value;

    private Fixed24d8(int v)
    {
        _value = v;
    }

    public float ToFloat()
    {
        return _value;
    }

    public static explicit operator int(Fixed24d8 f)
    {
        return f._value >> Bits;
    }

    public static implicit operator Fixed24d8(int v)
    {
        return new Fixed24d8(v << Bits);
    }
    
    public bool Equals(Fixed24d8 other)
    {
        return _value == other._value;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return ToFloat().ToString(format, formatProvider);
    }

    public override string ToString()
    {
        return ToFloat().ToString("F8");
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Fixed24d8 result)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Fixed24d8 result)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 operator +(Fixed24d8 left, Fixed24d8 right)
    {
        return new Fixed24d8(left._value + right._value);
    }

    public static Fixed24d8 AdditiveIdentity { get; }
    
    public static Fixed24d8 operator --(Fixed24d8 value)
    {
        return value - One;
    }

    public static Fixed24d8 operator /(Fixed24d8 left, Fixed24d8 right)
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(Fixed24d8 left, Fixed24d8 right)
    {
        return left._value == right._value;
    }

    public static bool operator !=(Fixed24d8 left, Fixed24d8 right)
    {
        return !(left == right);
    }

    public static Fixed24d8 operator ++(Fixed24d8 value)
    {
        return value + One;
    }

    public static Fixed24d8 MultiplicativeIdentity { get; }
    
    public static Fixed24d8 operator *(Fixed24d8 left, Fixed24d8 right)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 operator -(Fixed24d8 left, Fixed24d8 right)
    {
        return new Fixed24d8(left._value - right._value);
    }

    public static Fixed24d8 operator -(Fixed24d8 value)
    {
        return new Fixed24d8(-value._value);
    }

    public static Fixed24d8 operator +(Fixed24d8 value)
    {
        return new Fixed24d8(+value._value);
    }

    public static Fixed24d8 Abs(Fixed24d8 value)
    {
        return new Fixed24d8(Math.Abs(value._value));
    }

    public static bool IsCanonical(Fixed24d8 value) => true; // I guess?

    public static bool IsComplexNumber(Fixed24d8 value) => false;

    public static bool IsEvenInteger(Fixed24d8 value)
    {
        return IsInteger(value) && (int)value % 2 == 0;
    }

    public static bool IsFinite(Fixed24d8 value) => true;

    public static bool IsImaginaryNumber(Fixed24d8 value) => false;

    public static bool IsInfinity(Fixed24d8 value) => false;

    public static bool IsInteger(Fixed24d8 value)
    {
        return (value._value & 0xff) == 0;
    }

    public static bool IsNaN(Fixed24d8 value) => false;

    public static bool IsNegative(Fixed24d8 value)
    {
        return value._value < 0;
    }

    public static bool IsNegativeInfinity(Fixed24d8 value) => false;

    public static bool IsNormal(Fixed24d8 value)
    {
        throw new NotImplementedException();
    }

    public static bool IsOddInteger(Fixed24d8 value)
    {
        return IsInteger(value) && (int)value % 2 == 1;
    }

    public static bool IsPositive(Fixed24d8 value)
    {
        return value._value > 0;
    }

    public static bool IsPositiveInfinity(Fixed24d8 value) => false;

    public static bool IsRealNumber(Fixed24d8 value) => true;

    public static bool IsSubnormal(Fixed24d8 value)
    {
        throw new NotImplementedException();
    }

    public static bool IsZero(Fixed24d8 value)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 MaxMagnitude(Fixed24d8 x, Fixed24d8 y)
    {
        if (x._value > y._value)
            return x;
        else return y;
    }

    public static Fixed24d8 MaxMagnitudeNumber(Fixed24d8 x, Fixed24d8 y) => MaxMagnitude(x, y);

    public static Fixed24d8 MinMagnitude(Fixed24d8 x, Fixed24d8 y)
    {
        if (x._value < y._value)
            return x;
        else return y;
    }

    public static Fixed24d8 MinMagnitudeNumber(Fixed24d8 x, Fixed24d8 y) => MinMagnitude(x, y);

    public static Fixed24d8 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromChecked<TOther>(TOther value, out Fixed24d8 result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Fixed24d8 result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Fixed24d8 result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToChecked<TOther>(Fixed24d8 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToSaturating<TOther>(Fixed24d8 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryConvertToTruncating<TOther>(Fixed24d8 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Fixed24d8 result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Fixed24d8 result)
    {
        throw new NotImplementedException();
    }

    public static Fixed24d8 One { get; } = new(1 << Bits);
    public static int Radix { get; } = Bits;
    public static Fixed24d8 Zero { get; } = new(0);
    public static Fixed24d8 NegativeOne { get; } = new(-(1 << Bits));
}