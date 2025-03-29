using System.Drawing;
using JetBrains.Annotations;
using Radish.Logging;
using Radish.Platform;

namespace Radish.Input;

[PublicAPI]
public sealed class Gamepad
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<Gamepad>();
    
    private static readonly Dictionary<uint, Gamepad> Gamepads = [];
    public static int Count => Gamepads.Count;
    public static int CurrentActiveGamepadIndex { get; private set; } = -1;
    public static IEnumerable<Gamepad> All => Gamepads.Values;
    
    private static IPlatformBackend _platform = null!;

    private Gamepad()
    {
    }

    public string Name => _platform.GetGamepadName(Id) ?? string.Empty;
    public GamepadType Type => _platform.GetGamepadType(Id);
    public int PlayerIndex
    {
        get => _platform.GetGamepadPlayerIndex(Id);
        set => _platform.SetGamepadPlayerIndex(Id, value >= 0 ? value : -1);
    }
    public uint Id { get; private set; }
    public GamepadState State => _state;
    public bool Connected { get; private set; }
    
    private GamepadState _state;
    private float[] _rumbleValues = new float[4];
    
    public void SetLightColor(Color color) => _platform.SetGamepadLED(Id, color);

    public void SetRumble(GamepadRumbleMotor motor, float value)
    {
        _rumbleValues[(int)motor] = value;
        SendRumblePacket();
    }

    public void StopRumble()
    {
        for (var i = 0; i < _rumbleValues.Length; ++i)
            _rumbleValues[i] = 0;
        SendRumblePacket();
    }

    private void SendRumblePacket()
    {
        _platform.SendGamepadRumblePacket(Id, new GamepadRumbleState
        {
            LargeMotor = _rumbleValues[(int)GamepadRumbleMotor.LargeMotor],
            SmallMotor = _rumbleValues[(int)GamepadRumbleMotor.SmallMotor],
            LeftTrigger = _rumbleValues[(int)GamepadRumbleMotor.LeftTrigger],
            RightTrigger = _rumbleValues[(int)GamepadRumbleMotor.RightTrigger]
        });
    }

    internal static void SetPlatformBackend(IPlatformBackend platform)
    {
        _platform = platform;
    }

    public static GamepadState GetState(uint index)
    {
        if (Gamepads.TryGetValue(index, out var gs))
            return gs._state;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    public static Gamepad GetGamepad(uint index)
    {
        if (Gamepads.TryGetValue(index, out var gs))
            return gs;
        throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    public static bool TryGetGamepadWithPlayerIndex(int playerIndex, out uint id)
    {
        foreach (var (_, gp) in Gamepads)
        {
            if (gp.PlayerIndex != playerIndex) 
                continue;
            
            id = gp.Id;
            return true;
        }

        id = 0;
        return false;
    }
    
    private static Gamepad AddGamepadWithIndex(uint index)
    {
        if (Gamepads.TryGetValue(index, out var gp)) 
            return gp;
        gp = new Gamepad
        {
            Id = index
        };
        Gamepads.Add(index, gp);
        return gp;
    }

    internal static void NotifyGamepadAdded(uint id, GamepadType type)
    {
        var gs = AddGamepadWithIndex(id);
        gs.Connected = true;
        Logger.Info("Gamepad connected: {0} ({1})", gs.Name, gs.Id);
        Logger.Info("Player index: {0}", gs.PlayerIndex);
        Logger.Info("Type: {0}", gs.Type);
    }
    
    internal static void NotifyGamepadRemoved(uint id)
    {
        if (Gamepads.TryGetValue(id, out var gp))
        {
            gp.Connected = false;
            Gamepads.Remove(id);
            Logger.Info("Gamepad disconnected: {0}", id);
            if (CurrentActiveGamepadIndex == id)
                CurrentActiveGamepadIndex = -1;
        }
    }

    internal static void NotifyButtonDown(uint id, GamepadButton button)
    {
        if (Gamepads.TryGetValue(id, out var gp))
        {
            gp._state[button] = ButtonState.Down;
            CurrentActiveGamepadIndex = (int)id;
        }
    }

    internal static void NotifyButtonUp(uint id, GamepadButton button)
    {
        if (Gamepads.TryGetValue(id, out var gp))
        {
            gp._state[button] = ButtonState.Up;
        }
    }

    internal static void NotifyAxisChanged(uint id, GamepadAxis axis, float value)
    {
        if (Gamepads.TryGetValue(id, out var gp))
        {
            gp._state[axis] = value;
        }
    }

    internal static void NotifyRemapped(uint id, GamepadType type)
    {
        if (Gamepads.TryGetValue(id, out var gp))
        {
            Logger.Info("Gamepad remapped: {0}", gp.Name);
        }
    }
}