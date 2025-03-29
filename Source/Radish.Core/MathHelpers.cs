using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Radish;

[PublicAPI]
public static class MathHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b) where T : INumber<T>
    {
        return T.Min(a, b);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Max<T>(T a, T b) where T : INumber<T>
    {
        return T.Max(a, b);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(T v, T min, T max) where T : INumber<T>
    {
        return T.Clamp(v, min, max);
    }
}