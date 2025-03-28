using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Radish.Numerics;

/// <summary>
/// A 2d vector with a layout that supports directly copying into graphics memory.
/// </summary>
/// <param name="x">The vector X component.</param>
/// <param name="y">The vector Y component.</param>
[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
[DebuggerDisplay("[{X}, {Y}]")]
public struct Float2(float x, float y)
{
    public float X = x;
    public float Y = y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2(Float2 f)
    {
        return new Vector2(f.X, f.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float2(Vector2 f)
    {
        return new Float2(f.X, f.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float2(Size f)
    {
        return new Float2(f.Width, f.Height);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float2(SizeF f)
    {
        return new Float2(f.Width, f.Height);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float2(Point f)
    {
        return new Float2(f.X, f.Y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float2(PointF f)
    {
        return new Float2(f.X, f.Y);
    }

    public override string ToString()
    {
        return $"[ {X}, {Y} ]";
    }
}