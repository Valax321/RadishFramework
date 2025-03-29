using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Radish.Input;

[PublicAPI]
public unsafe struct MouseState
{
    public Vector2 Position { get; internal set; }
    public Vector2 Wheel { get; internal set; }
    private fixed byte _buttons[5];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ButtonState GetButton(MouseButton button)
    {
        return (ButtonState)_buttons[(int)button];
    }

    public ButtonState this[MouseButton index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetButton(index);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _buttons[(int)index] = (byte)value;
    }
}