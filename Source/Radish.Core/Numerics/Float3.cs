using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Radish.Numerics;

/// <summary>
/// A 3d vector with a layout that supports directly copying into graphics memory.
/// </summary>
/// <param name="x">The vector X component.</param>
/// <param name="y">The vector Y component.</param>
/// <param name="z">The vector Z component.</param>
[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("[{X}, {Y}, {Z}]")]
public struct Float3(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3(Float3 f)
    {
        return new Vector3(f.X, f.Y, f.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float3(Vector3 f)
    {
        return new Float3(f.X, f.Y, f.Z);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float3(Color c)
    {
        return new Float3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
    }

    public override string ToString()
    {
        return $"[ {X}, {Y}, {Z} ]";
    }
}