using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Text;
using JetBrains.Annotations;

namespace Radish.Text;

[PublicAPI]
public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimEnd(this string s, string text)
    {
        var l = text.Length;
        if (s.EndsWith(text))
            return s[..^l];
        return s;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReplaceAll(this string s, char replacement, params ReadOnlySpan<char> chars)
    {
        using var sb = ZString.CreateStringBuilder();
        foreach (var c in s)
        {
            sb.Append(chars.Contains(c) ? replacement : c);
        }

        return sb.ToString();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RemoveAll(this string s, params ReadOnlySpan<char> chars)
    {
        using var sb = ZString.CreateStringBuilder();
        foreach (var c in s)
        {
            if (chars.Contains(c))
                continue;
            sb.Append(c);
        }

        return sb.ToString();
    }

    public static unsafe uint HashStable(this string s)
    {
        // Since we don't know how long the input string might be,
        // use a heap memory buffer to be safe.
        var length = Encoding.UTF8.GetByteCount(s);
        var data = (byte*)NativeMemory.Alloc((UIntPtr)length);
        Encoding.UTF8.GetBytes(s, new Span<byte>(data, length));
        var h = Crc32.HashToUInt32(new ReadOnlySpan<byte>(data, length));
        NativeMemory.Free(data);
        return h;
    }
}