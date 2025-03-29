using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Radish.Input;

[PublicAPI]
public unsafe struct KeyboardState
{
    private fixed byte _keys[255];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ButtonState GetKey(Keys key)
    {
        return (ButtonState)_keys[(int)key];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyDown(Keys key)
    {
        return GetKey(key) == ButtonState.Down;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsKeyUp(Keys key)
    {
        return GetKey(key) == ButtonState.Up;
    }
    
    public ButtonState this[Keys key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetKey(key);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _keys[(int)key] = (byte)value;
    }
}