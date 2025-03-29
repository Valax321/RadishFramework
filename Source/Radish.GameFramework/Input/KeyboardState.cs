using JetBrains.Annotations;

namespace Radish.Input;

[PublicAPI]
public unsafe struct KeyboardState
{
    private fixed byte _keys[255];
    
    public ButtonState GetKey(Keys key)
    {
        return (ButtonState)_keys[(int)key];
    }
    
    public ButtonState this[Keys key]
    {
        get => GetKey(key);
        internal set => _keys[(int)key] = (byte)value;
    }
}