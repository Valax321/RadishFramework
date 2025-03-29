using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Radish.Input;

[PublicAPI]
public unsafe struct GamepadState
{
    // Be sure to increase this if GamepadButton or GamepadAxis grows or things will go boom
    private fixed byte _buttons[17];
    private fixed float _axes[10];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ButtonState GetButton(GamepadButton button)
    {
        return (ButtonState)_buttons[(int)button];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsButtonDown(GamepadButton button)
    {
        return GetButton(button) == ButtonState.Down;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsButtonUp(GamepadButton button)
    {
        return GetButton(button) == ButtonState.Up;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetAxis(GamepadAxis axis)
    {
        return _axes[(int)axis];
    }

    public ButtonState this[GamepadButton button]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetButton(button);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _buttons[(int)button] = (byte)value;
    }

    public float this[GamepadAxis axis]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetAxis(axis);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _axes[(int)axis] = value;
    }
}