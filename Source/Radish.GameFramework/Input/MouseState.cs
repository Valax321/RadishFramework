using System.Numerics;
using JetBrains.Annotations;

namespace Radish.Input;

[PublicAPI]
public unsafe struct MouseState
{
    public Vector2 Position { get; internal set; }
    public Vector2 Wheel { get; internal set; }
    private fixed byte _buttons[5];

    public ButtonState GetButton(MouseButton button)
    {
        return (ButtonState)_buttons[(int)button];
    }

    public ButtonState this[MouseButton index]
    {
        get => GetButton(index);
        internal set => _buttons[(int)index] = (byte)value;
    }
}