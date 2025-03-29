using System.Numerics;
using JetBrains.Annotations;
using Radish.Logging;
using Radish.Platform;

namespace Radish.Input;

[PublicAPI]
public sealed class Mouse
{
    public delegate void ClickedDelegate(uint id, MouseButton button, int clickCount);
    
    private static readonly ILogger Logger = LogManager.GetLoggerForType<Keyboard>();
    
    private static IPlatformBackend _platform = null!;
    private static readonly Dictionary<uint, Mouse> Mice = [];

    public static int Count => Mice.Count;
    public static int CurrentActiveMouseIndex { get; private set; } = -1;
    public static event ClickedDelegate OnClick
    {
        add => _onClick += value;
        remove
        {
            if (_onClick != null)
                _onClick -= value;
        }
    }
    
    public uint Id { get; private set; }
    public MouseState State => _state;
    public bool IsConnected { get; private set; }
    public string Name => _platform.GetMouseName(Id);
    
    private MouseState _state;
    private static ClickedDelegate? _onClick;

    private Mouse()
    { }

    public static MouseState GetState(uint index)
    {
        if (Mice.TryGetValue(index, out var kb))
            return kb._state;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    public static void SetPosition(int x, int y)
    {
        
    }
    
    public static Mouse GetMouse(uint index)
    {
        if (Mice.TryGetValue(index, out var kb))
            return kb;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }
    
    internal static void SetPlatformBackend(IPlatformBackend platform)
    {
        _platform = platform;
    }

    internal static void NotifyMouseAdded(uint id)
    {
        var m = AddMouseWithIndex(id);
        m.IsConnected = true;
        Logger.Info("Mouse connected: {0} ({1})", m.Name, m.Id);
    }
    
    private static Mouse AddMouseWithIndex(uint index)
    {
        if (Mice.TryGetValue(index, out var m)) 
            return m;
        m = new Mouse
        {
            Id = index
        };
        Mice.Add(index, m);
        return m;
    }

    internal static void NotifyMouseRemoved(uint id)
    {
        if (Mice.TryGetValue(id, out var m))
        {
            Logger.Info("Mouse disconnected: {0}", m.Id);
            m.IsConnected = false;
            Mice.Remove(id);
            if (CurrentActiveMouseIndex == id)
                CurrentActiveMouseIndex = -1;
        }
    }

    internal static void NotifyButtonDown(uint id, MouseButton button)
    {
        var m = AddMouseWithIndex(id);
        {
            m._state[button] = ButtonState.Down;
            CurrentActiveMouseIndex = (int)id;
        }
    }

    internal static void NotifyClick(uint id, MouseButton button, int clickCount)
    {
        var m = AddMouseWithIndex(id);
        _onClick?.Invoke(id, button, clickCount);
    }

    internal static void NotifyButtonUp(uint id, MouseButton button)
    {
        var m = AddMouseWithIndex(id);
        {
            m._state[button] = ButtonState.Up;
            CurrentActiveMouseIndex = (int)id;
        }
    }

    internal static void NotifyMouseMovement(uint id, Vector2 position)
    {
        var m = AddMouseWithIndex(id);
        {
            m._state.Position = position;
        }
    }
    
    internal static void NotifyMouseWheelPosition(uint id, Vector2 position)
    {
        var m = AddMouseWithIndex(id);
        {
            m._state.Wheel = position;
        }
    }
}