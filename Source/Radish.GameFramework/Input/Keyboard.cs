using JetBrains.Annotations;
using Radish.Logging;
using Radish.Platform;

namespace Radish.Input;

[PublicAPI]
public sealed class Keyboard
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<Keyboard>();
    
    private static IPlatformBackend _platform = null!;
    private static readonly Dictionary<uint, Keyboard> Keyboards = [];

    public static int Count => Keyboards.Count;
    public static int CurrentActiveKeyboardIndex { get; private set; } = -1;
    
    public uint Id { get; private set; }
    public KeyboardState State => _state;
    public bool IsConnected { get; private set; }
    public string Name => _platform.GetKeyboardName(Id);
    
    private KeyboardState _state;

    private Keyboard()
    { }

    public static KeyboardState GetState(uint index)
    {
        if (Keyboards.TryGetValue(index, out var kb))
            return kb._state;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
    
    public static Keyboard GetKeyboard(uint index)
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
        var kb = AddKeyboardWithIndex(id);
        kb.IsConnected = true;
        Logger.Info("Keyboard connected: {0} ({1})", kb.Name, kb.Id);
    }
    
    private static Keyboard AddKeyboardWithIndex(uint index)
    {
        if (Keyboards.TryGetValue(index, out var kb)) 
            return kb;
        kb = new Keyboard
        {
            Id = index
        };
        Keyboards.Add(index, kb);
        return kb;
    }

    internal static void NotifyKeyboardRemoved(uint id)
    {
        if (Keyboards.TryGetValue(id, out var kb))
        {
            Logger.Info("Keyboard disconnected: {0}", kb.Id);
            kb.IsConnected = false;
            Keyboards.Remove(id);
            if (CurrentActiveKeyboardIndex == id)
                CurrentActiveKeyboardIndex = -1;
        }
    }

    internal static void NotifyKeyDown(uint id, Keys scancode)
    {
        // Keyboard handling is a bit different since we don't seem to get keyboard
        // add events on all platforms...
        // just add the keyboard if its ID doesn't exist yet
        var kb = AddKeyboardWithIndex(id);
        {
            kb._state[scancode] = ButtonState.Down;
        }
    }

    internal static void NotifyKeyUp(uint id, Keys scancode)
    {
        var kb = AddKeyboardWithIndex(id);
        {
            kb._state[scancode] = ButtonState.Up;
        }
    }
}