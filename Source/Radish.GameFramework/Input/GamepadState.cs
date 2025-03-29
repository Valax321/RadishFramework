using JetBrains.Annotations;

namespace Radish.Input;

[PublicAPI]
public unsafe struct GamepadState
{
    // Be sure to increase this if GamepadButton or GamepadAxis grows or things will go boom
    private fixed byte _buttons[17];
    private fixed float _axes[10];

    public ButtonState GetButton(GamepadButton button)
    {
        return (ButtonState)_buttons[(int)button];
    }
    
    public float GetAxis(GamepadAxis axis)
    {
        return _axes[(int)axis];
    }

    public ButtonState this[GamepadButton button]
    {
        get => GetButton(button);
        internal set => _buttons[(int)button] = (byte)value;
    }

    public float this[GamepadAxis axis]
    {
        get => GetAxis(axis);
        internal set => _axes[(int)axis] = value;
    }
}