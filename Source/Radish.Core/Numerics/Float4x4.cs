using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Radish.Numerics;

[PublicAPI]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Float4x4
{
    public fixed float M[16];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Float4x4(Matrix4x4 m)
    {
        var f = new Float4x4();
        for (var i = 0; i < 16; ++i)
        {
            f.M[i] = m[i / 4, i % 4];
        }
        return f;
    }
}