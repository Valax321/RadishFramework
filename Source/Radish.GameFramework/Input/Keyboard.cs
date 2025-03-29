using JetBrains.Annotations;
using Radish.Platform;

namespace Radish.Input;

[PublicAPI]
public sealed class Keyboard
{
    private static IPlatformBackend _platform = null!;
    private static readonly Dictionary<int, Keyboard> Keyboards = [];
    
    public static int Count { get; private set; }
    public static int CurrentActiveKeyboardIndex { get; private set; } = -1;
    
    public uint Id { get; private set; }
    public KeyboardState State => _state;
    public bool IsConnected { get; private set; }
    
    private KeyboardState _state;

    private Keyboard()
    { }

    public static KeyboardState GetState(int index)
    {
        if (Keyboards.TryGetValue(index, out var kb))
            return kb._state;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
    
    public static Keyboard GetGamepad(int index)
    {
        if (Keyboards.TryGetValue(index, out var kb))
            return kb;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    public static Keys ScancodeToKeycode(Keys key) => _platform.ScancodeToKeycode(key);
    
    internal static void SetPlatformBackend(IPlatformBackend platform)
    {
        _platform = platform;
    }

    internal static void NotifyKeyboardAdded(uint id)
    {
        var kb = AddKeyboardWithIndex((int)id);
        kb.Id = id;
        kb.IsConnected = true;
        Count++;
    }
    
    private static Keyboard AddKeyboardWithIndex(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        
        if (Keyboards.TryGetValue(index, out var kb)) 
            return kb;
        kb = new Keyboard();
        Keyboards.Add(index, kb);
        return kb;
    }

    internal static void NotifyKeyboardRemoved(uint id)
    {
        if (Keyboards.TryGetValue((int)id, out var kb))
        {
            kb.IsConnected = false;
        }
        Count--;
    }

    internal static void NotifyKeyDown(uint id, Keys scancode)
    {
        
    }

    internal static void NotifyKeyUp(uint id, Keys scancode)
    {
        
    }
}