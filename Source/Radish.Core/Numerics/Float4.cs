using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Radish.Numerics;

/// <summary>
/// A 4d vector with a layout that supports directly copying into graphics memory.
/// </summary>
/// <param name="x">The vector X component.</param>
/// <param name="y">The vector Y component.</param>
/// <param name="z">The vector Z component.</param>
/// <param name="w">The vector W component.</param>
[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("[{X}, {Y}, {Z}, {W}]")]
public struct Float4(float x, float y, float z, float w)
{
    public float X = x;
    public float Y = y;
    public float Z = z;
    public float W = w;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4(Float4 f)
    {
        return new Vector4(f.X, f.Y, f.Z, f.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float4(Vector4 f)
    {
        return new Float4(f.X, f.Y, f.Z, f.W);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float4(Color c)
    {
        return new Float4(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f, c.A / 255.0f);
    }

    public override string ToString()
    {
        return $"[ {X}, {Y}, {Z}, {W} ]";
    }
}