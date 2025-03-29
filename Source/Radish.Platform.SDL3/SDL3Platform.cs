using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using JetBrains.Annotations;
using Radish.Graphics;
using Radish.Input;
using Radish.Logging;
using Radish.Text;
using static SDL3.SDL;

namespace Radish.Platform;

public sealed unsafe partial class SDL3Platform : IPlatformBackend
{
    private static readonly ILogger Logger = LogManager.GetLoggerForType<SDL3Platform>();

    static SDL3Platform()
    {
        NativeLibraryResolver.InitializeForAssembly(Assembly.GetExecutingAssembly());
    }

    [PublicAPI]
    public static SDL3Platform Create() => new();

    public string Name => "SDL3";
    public bool WantsToQuit { get; private set; }

    private SDL3Platform()
    {
        SDL_SetLogOutputFunction(OnSDL_Log, IntPtr.Zero);
        SDL_SetLogPriorities(SDL_LogPriority.SDL_LOG_PRIORITY_WARN);
        
        var assembly = Assembly.GetEntryAssembly();
        if (assembly is not null)
        {
            var product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            var version = assembly.GetCustomAttribute<AssemblyVersionAttribute>();
            var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            var name = assembly.GetName();

            var identifier = $"com.{company?.Company.ToLower().RemoveAll(' ')}.{name.Name?.ToLower().RemoveAll(' ')}";

            SDL_SetAppMetadata(
                product?.Product ?? string.Empty,
                version?.Version ?? string.Empty,
                identifier);
            SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_TYPE_STRING, "game");

            if (company is not null)
                SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING, company.Company);
            if (copyright is not null)
                SDL_SetAppMetadataProperty(SDL_PROP_APP_METADATA_COPYRIGHT_STRING, copyright.Copyright);
        }

        var success = SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_GAMEPAD |
                               SDL_InitFlags.SDL_INIT_HAPTIC |
                               SDL_InitFlags.SDL_INIT_SENSOR);
        if (!success)
            throw new PlatformException($"SDL_Init failed: {SDL_GetError()}");

        var v = SDL_GetVersion();
        Logger.Info("SDL version: {0}.{1}.{2}", v / 1000000, (v / 1000) % 1000, (v % 1000));
    }

    private void OnSDL_Log(IntPtr userdata, int category, SDL_LogPriority priority, byte* message)
    {
        var msg = Utf8StringMarshaller.ConvertToManaged(message);
        switch (priority)
        {
            case SDL_LogPriority.SDL_LOG_PRIORITY_TRACE:
            case SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE:
            case SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG:
            case SDL_LogPriority.SDL_LOG_PRIORITY_INFO:
                Logger.Info("{0}", msg);
                break;
            case SDL_LogPriority.SDL_LOG_PRIORITY_WARN:
                Logger.Warn("{0}", msg);
                break;
            case SDL_LogPriority.SDL_LOG_PRIORITY_ERROR:
                Logger.Error("{0}", msg);
                break;
            case SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL:
                Logger.Error("{0}", msg);
                break;
            default:
                return;
        }
    }

    public void Dispose()
    {
        var pads = (void**)SDL_GetGamepads(out var count);
        for (var i = 0; i < count; ++i)
            SDL_CloseGamepad((IntPtr)pads[i]);
        SDL_Quit();
    }

    public void PumpEvents()
    {
        WantsToQuit = false;
        while (SDL_PollEvent(out var ev))
        {
            ProcessEvent(ev);
        }
    }

    private void ProcessEvent(in SDL_Event ev)
    {
        switch ((SDL_EventType)ev.type)
        {
            case SDL_EventType.SDL_EVENT_QUIT:
                WantsToQuit = true;
                break;

            #region Graphics

            case SDL_EventType.SDL_EVENT_WINDOW_PIXEL_SIZE_CHANGED:
            case SDL_EventType.SDL_EVENT_WINDOW_DISPLAY_CHANGED:
            {
                var wnd = SDL_GetWindowFromID(ev.window.windowID);
                SDL_GetWindowSizeInPixels(wnd, out var w, out var h);
                GraphicsDeviceManager.Reset(wnd, new Size(w, h), SDL_GetDisplayForWindow(wnd));
            }
                break;
            #endregion

            #region Gamepads

            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
            {
                var gamepad = SDL_OpenGamepad(ev.gdevice.which);
                var id = SDL_GetGamepadID(gamepad);
                if (SDL_GamepadHasSensor(gamepad, SDL_SensorType.SDL_SENSOR_GYRO))
                    SDL_SetGamepadSensorEnabled(gamepad, SDL_SensorType.SDL_SENSOR_GYRO, true);
                Gamepad.NotifyGamepadAdded(id, MapSDLGamepadTypeToGame(SDL_GetGamepadType(gamepad)));
            }
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
            {
                Gamepad.NotifyGamepadRemoved(ev.gdevice.which);
                var gamepad = SDL_GetGamepadFromID(ev.gdevice.which);
                SDL_CloseGamepad(gamepad);
            }
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMAPPED:
            {
                var gamepad = SDL_GetGamepadFromID(ev.gdevice.which);
                Gamepad.NotifyRemapped(ev.gdevice.which,
                    MapSDLGamepadTypeToGame(SDL_GetGamepadType(gamepad)));
            }
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
                Gamepad.NotifyButtonDown(ev.gbutton.which,
                    MapSDLGamepadButtonToGame((SDL_GamepadButton)ev.gbutton.button));
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                Gamepad.NotifyButtonUp(ev.gbutton.which,
                    MapSDLGamepadButtonToGame((SDL_GamepadButton)ev.gbutton.button));
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
            {
                var axis = MapSDLGamepadAxisToGame((SDL_GamepadAxis)ev.gaxis.axis);
                var id = ev.gaxis.which;
                Gamepad.NotifyAxisChanged(id, axis, (float)ev.gaxis.value / (float)short.MaxValue);
            }
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_SENSOR_UPDATE:
            {
                if ((SDL_SensorType)ev.gsensor.sensor != SDL_SensorType.SDL_SENSOR_GYRO)
                    break;

                var xVal = ev.gsensor.data[0];
                var yVal = ev.gsensor.data[1];
                var zVal = ev.gsensor.data[2];
                Gamepad.NotifyAxisChanged(ev.gsensor.which, GamepadAxis.GyroX, xVal);
                Gamepad.NotifyAxisChanged(ev.gsensor.which, GamepadAxis.GyroY, yVal);
                Gamepad.NotifyAxisChanged(ev.gsensor.which, GamepadAxis.GyroZ, zVal);
            }
                break;

            #endregion

            #region Keyboards

            case SDL_EventType.SDL_EVENT_KEYBOARD_ADDED:
            {
                Keyboard.NotifyKeyboardAdded(ev.key.which);
            }
                break;
            case SDL_EventType.SDL_EVENT_KEYBOARD_REMOVED:
            {
                Keyboard.NotifyKeyboardRemoved(ev.key.which);
            }
                break;
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
            {
                Keyboard.NotifyKeyDown(ev.key.which, ScancodesToKeys.GetValueOrDefault((int)ev.key.scancode, Keys.None));
            }
                break;
            case SDL_EventType.SDL_EVENT_KEY_UP:
            {
                Keyboard.NotifyKeyUp(ev.key.which, ScancodesToKeys.GetValueOrDefault((int)ev.key.scancode, Keys.None));
            }
                break;

            #endregion

            #region Mice

            case SDL_EventType.SDL_EVENT_MOUSE_ADDED:
            {
                Mouse.NotifyMouseAdded(ev.mdevice.which);
            }
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_REMOVED:
            {
                Mouse.NotifyMouseRemoved(ev.mdevice.which);
            }
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
            {
                Mouse.NotifyButtonDown(ev.button.which, MapSDLMouseButtonToGame((SDL_MouseButtonFlags)ev.button.button));
            }
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
            {
                Mouse.NotifyButtonUp(ev.button.which, MapSDLMouseButtonToGame((SDL_MouseButtonFlags)ev.button.button));
                
                Mouse.NotifyClick(ev.button.which, MapSDLMouseButtonToGame((SDL_MouseButtonFlags)ev.button.button), 
                    ev.button.clicks);
            }
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
            {
                Mouse.NotifyMouseMovement(ev.motion.which, new Vector2(ev.motion.x, ev.motion.y));
            }
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
            {
                Mouse.NotifyMouseWheelPosition(ev.wheel.which, new Vector2(ev.wheel.x, ev.wheel.y));
            }
                break;

            #endregion
        }
    }
    
    public GamepadType GetGamepadType(uint id)
    {
        return MapSDLGamepadTypeToGame(SDL_GetGamepadTypeForID(id));
    }

    public string? GetGamepadName(uint id)
    {
        return SDL_GetGamepadNameForID(id);
    }

    public void SetGamepadPlayerIndex(uint id, int index)
    {
        var gamepad = SDL_GetGamepadFromID(id);
        if (gamepad != IntPtr.Zero)
        {
            SDL_SetGamepadPlayerIndex(gamepad, index);
        }
    }

    public int GetGamepadPlayerIndex(uint id)
    {
        return SDL_GetGamepadPlayerIndexForID(id);
    }

    public void SetGamepadLED(uint id, Color color)
    {
        var gamepad = SDL_GetGamepadFromID(id);
        if (gamepad != IntPtr.Zero)
        {
            SDL_SetGamepadLED(gamepad, color.R, color.G, color.B);
        }
    }

    public void SendGamepadRumblePacket(uint id, in GamepadRumbleState state)
    {
        var gamepad = SDL_GetGamepadFromID(id);
        if (gamepad != IntPtr.Zero)
        {
            SDL_RumbleGamepad(gamepad,
                (ushort)(float.Clamp(state.LargeMotor, 0.0f, 1.0f) / ushort.MaxValue),
                (ushort)(float.Clamp(state.SmallMotor, 0.0f, 1.0f) / ushort.MaxValue),
                uint.MaxValue);

            SDL_RumbleGamepadTriggers(gamepad,
                (ushort)(float.Clamp(state.LeftTrigger, 0.0f, 1.0f) / ushort.MaxValue),
                (ushort)(float.Clamp(state.RightTrigger, 0.0f, 1.0f) / ushort.MaxValue),
                uint.MaxValue);
        }
    }

    public string GetGamepadSerial(uint id)
    {
        var gamepad = SDL_GetGamepadFromID(id);
        if (gamepad == IntPtr.Zero)
            return string.Empty;
        return SDL_GetGamepadSerial(gamepad);
    }

    private static MouseButton MapSDLMouseButtonToGame(SDL_MouseButtonFlags button)
    {
        return button switch
        {
            SDL_MouseButtonFlags.SDL_BUTTON_LMASK => MouseButton.Left,
            SDL_MouseButtonFlags.SDL_BUTTON_MMASK => MouseButton.Middle,
            SDL_MouseButtonFlags.SDL_BUTTON_RMASK => MouseButton.Right,
            SDL_MouseButtonFlags.SDL_BUTTON_X1MASK => MouseButton.Mouse4,
            SDL_MouseButtonFlags.SDL_BUTTON_X2MASK => MouseButton.Mouse5,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
        };
    }

    private static GamepadButton MapSDLGamepadButtonToGame(SDL_GamepadButton button)
    {
        return button switch
        {
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH => GamepadButton.FaceSouth,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_EAST => GamepadButton.FaceEast,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_WEST => GamepadButton.FaceWest,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_NORTH => GamepadButton.FaceNorth,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_BACK => GamepadButton.Back,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_GUIDE => GamepadButton.Guide,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START => GamepadButton.Options,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_STICK => GamepadButton.LeftStickPress,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_STICK => GamepadButton.RightStickPress,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_SHOULDER => GamepadButton.LeftShoulder,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER => GamepadButton.RightShoulder,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP => GamepadButton.DPadUp,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN => GamepadButton.DPadDown,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_LEFT => GamepadButton.DPadLeft,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_RIGHT => GamepadButton.DPadRight,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_TOUCHPAD => GamepadButton.Touchpad,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_PADDLE1 => GamepadButton.LeftPaddle1,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_LEFT_PADDLE2 => GamepadButton.LeftPaddle2,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_PADDLE1 => GamepadButton.RightPaddle1,
            SDL_GamepadButton.SDL_GAMEPAD_BUTTON_RIGHT_PADDLE2 => GamepadButton.RightPaddle2,
            _ => GamepadButton.Unknown
        };
    }

    private static GamepadAxis MapSDLGamepadAxisToGame(SDL_GamepadAxis axis)
    {
        return axis switch
        {
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX => GamepadAxis.LeftStickX,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY => GamepadAxis.LeftStickY,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX => GamepadAxis.RightStickX,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY => GamepadAxis.RightStickY,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER => GamepadAxis.LeftTrigger,
            SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER => GamepadAxis.RightTrigger,
            _ => GamepadAxis.Unknown
        };
    }

    private static GamepadType MapSDLGamepadTypeToGame(SDL_GamepadType type)
    {
        return type switch
        {
            SDL_GamepadType.SDL_GAMEPAD_TYPE_STANDARD => GamepadType.Unknown,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOX360 => GamepadType.Xbox360,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_XBOXONE => GamepadType.XboxOne,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_PS3 => GamepadType.Dualshock3,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_PS4 => GamepadType.Dualshock4,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5 => GamepadType.Dualsense,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO => GamepadType.Switch,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_LEFT => GamepadType.SwitchJoyconLeft,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_RIGHT => GamepadType.SwitchJoyconRight,
            SDL_GamepadType.SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_PAIR => GamepadType.Switch,
            _ => GamepadType.Unknown
        };
    }
    
    public Keys ScancodeToKeycode(Keys key)
    {
        if (!KeysToScancodes.TryGetValue((int)key, out var scancode))
            return Keys.None;

        var keycode = SDL_GetKeyFromScancode(scancode, SDL_Keymod.SDL_KMOD_NONE, false);
        return KeycodesToKeys.GetValueOrDefault((int)keycode, Keys.None);
    }

    public string GetKeyboardName(uint id)
    {
        var name = SDL_GetKeyboardNameForID(id);
        if (name == null!)
            Logger.Warn("SDL_GetKeyboardNameForID error: {0}", SDL_GetError());
        return name ?? string.Empty;
    }

    public string GetMouseName(uint id)
    {
        var name = SDL_GetMouseNameForID(id);
        if (name == null!)
            Logger.Warn("SDL_GetMouseNameForID error: {0}", SDL_GetError());
        return name ?? string.Empty;
    }

    // I have taken these ungodly lookup tables from FNA since we're using XNA's Keys enum too,
    // might as well save the pain of putting this together ourselves.
    private static Dictionary<int, Keys> KeycodesToKeys = new()
    {
        { (int)SDL.SDL_Keycode.SDLK_A, Keys.A },
        { (int)SDL.SDL_Keycode.SDLK_B, Keys.B },
        { (int)SDL.SDL_Keycode.SDLK_C, Keys.C },
        { (int)SDL.SDL_Keycode.SDLK_D, Keys.D },
        { (int)SDL.SDL_Keycode.SDLK_E, Keys.E },
        { (int)SDL.SDL_Keycode.SDLK_F, Keys.F },
        { (int)SDL.SDL_Keycode.SDLK_G, Keys.G },
        { (int)SDL.SDL_Keycode.SDLK_H, Keys.H },
        { (int)SDL.SDL_Keycode.SDLK_I, Keys.I },
        { (int)SDL.SDL_Keycode.SDLK_J, Keys.J },
        { (int)SDL.SDL_Keycode.SDLK_K, Keys.K },
        { (int)SDL.SDL_Keycode.SDLK_L, Keys.L },
        { (int)SDL.SDL_Keycode.SDLK_M, Keys.M },
        { (int)SDL.SDL_Keycode.SDLK_N, Keys.N },
        { (int)SDL.SDL_Keycode.SDLK_O, Keys.O },
        { (int)SDL.SDL_Keycode.SDLK_P, Keys.P },
        { (int)SDL.SDL_Keycode.SDLK_Q, Keys.Q },
        { (int)SDL.SDL_Keycode.SDLK_R, Keys.R },
        { (int)SDL.SDL_Keycode.SDLK_S, Keys.S },
        { (int)SDL.SDL_Keycode.SDLK_T, Keys.T },
        { (int)SDL.SDL_Keycode.SDLK_U, Keys.U },
        { (int)SDL.SDL_Keycode.SDLK_V, Keys.V },
        { (int)SDL.SDL_Keycode.SDLK_W, Keys.W },
        { (int)SDL.SDL_Keycode.SDLK_X, Keys.X },
        { (int)SDL.SDL_Keycode.SDLK_Y, Keys.Y },
        { (int)SDL.SDL_Keycode.SDLK_Z, Keys.Z },
        { (int)SDL.SDL_Keycode.SDLK_0, Keys.D0 },
        { (int)SDL.SDL_Keycode.SDLK_1, Keys.D1 },
        { (int)SDL.SDL_Keycode.SDLK_2, Keys.D2 },
        { (int)SDL.SDL_Keycode.SDLK_3, Keys.D3 },
        { (int)SDL.SDL_Keycode.SDLK_4, Keys.D4 },
        { (int)SDL.SDL_Keycode.SDLK_5, Keys.D5 },
        { (int)SDL.SDL_Keycode.SDLK_6, Keys.D6 },
        { (int)SDL.SDL_Keycode.SDLK_7, Keys.D7 },
        { (int)SDL.SDL_Keycode.SDLK_8, Keys.D8 },
        { (int)SDL.SDL_Keycode.SDLK_9, Keys.D9 },
        { (int)SDL.SDL_Keycode.SDLK_KP_0, Keys.NumPad0 },
        { (int)SDL.SDL_Keycode.SDLK_KP_1, Keys.NumPad1 },
        { (int)SDL.SDL_Keycode.SDLK_KP_2, Keys.NumPad2 },
        { (int)SDL.SDL_Keycode.SDLK_KP_3, Keys.NumPad3 },
        { (int)SDL.SDL_Keycode.SDLK_KP_4, Keys.NumPad4 },
        { (int)SDL.SDL_Keycode.SDLK_KP_5, Keys.NumPad5 },
        { (int)SDL.SDL_Keycode.SDLK_KP_6, Keys.NumPad6 },
        { (int)SDL.SDL_Keycode.SDLK_KP_7, Keys.NumPad7 },
        { (int)SDL.SDL_Keycode.SDLK_KP_8, Keys.NumPad8 },
        { (int)SDL.SDL_Keycode.SDLK_KP_9, Keys.NumPad9 },
        { (int)SDL.SDL_Keycode.SDLK_KP_CLEAR, Keys.OemClear },
        { (int)SDL.SDL_Keycode.SDLK_KP_DECIMAL, Keys.Decimal },
        { (int)SDL.SDL_Keycode.SDLK_KP_DIVIDE, Keys.Divide },
        { (int)SDL.SDL_Keycode.SDLK_KP_ENTER, Keys.Enter },
        { (int)SDL.SDL_Keycode.SDLK_KP_MINUS, Keys.Subtract },
        { (int)SDL.SDL_Keycode.SDLK_KP_MULTIPLY, Keys.Multiply },
        { (int)SDL.SDL_Keycode.SDLK_KP_PERIOD, Keys.OemPeriod },
        { (int)SDL.SDL_Keycode.SDLK_KP_PLUS, Keys.Add },
        { (int)SDL.SDL_Keycode.SDLK_F1, Keys.F1 },
        { (int)SDL.SDL_Keycode.SDLK_F2, Keys.F2 },
        { (int)SDL.SDL_Keycode.SDLK_F3, Keys.F3 },
        { (int)SDL.SDL_Keycode.SDLK_F4, Keys.F4 },
        { (int)SDL.SDL_Keycode.SDLK_F5, Keys.F5 },
        { (int)SDL.SDL_Keycode.SDLK_F6, Keys.F6 },
        { (int)SDL.SDL_Keycode.SDLK_F7, Keys.F7 },
        { (int)SDL.SDL_Keycode.SDLK_F8, Keys.F8 },
        { (int)SDL.SDL_Keycode.SDLK_F9, Keys.F9 },
        { (int)SDL.SDL_Keycode.SDLK_F10, Keys.F10 },
        { (int)SDL.SDL_Keycode.SDLK_F11, Keys.F11 },
        { (int)SDL.SDL_Keycode.SDLK_F12, Keys.F12 },
        { (int)SDL.SDL_Keycode.SDLK_F13, Keys.F13 },
        { (int)SDL.SDL_Keycode.SDLK_F14, Keys.F14 },
        { (int)SDL.SDL_Keycode.SDLK_F15, Keys.F15 },
        { (int)SDL.SDL_Keycode.SDLK_F16, Keys.F16 },
        { (int)SDL.SDL_Keycode.SDLK_F17, Keys.F17 },
        { (int)SDL.SDL_Keycode.SDLK_F18, Keys.F18 },
        { (int)SDL.SDL_Keycode.SDLK_F19, Keys.F19 },
        { (int)SDL.SDL_Keycode.SDLK_F20, Keys.F20 },
        { (int)SDL.SDL_Keycode.SDLK_F21, Keys.F21 },
        { (int)SDL.SDL_Keycode.SDLK_F22, Keys.F22 },
        { (int)SDL.SDL_Keycode.SDLK_F23, Keys.F23 },
        { (int)SDL.SDL_Keycode.SDLK_F24, Keys.F24 },
        { (int)SDL.SDL_Keycode.SDLK_SPACE, Keys.Space },
        { (int)SDL.SDL_Keycode.SDLK_UP, Keys.Up },
        { (int)SDL.SDL_Keycode.SDLK_DOWN, Keys.Down },
        { (int)SDL.SDL_Keycode.SDLK_LEFT, Keys.Left },
        { (int)SDL.SDL_Keycode.SDLK_RIGHT, Keys.Right },
        { (int)SDL.SDL_Keycode.SDLK_LALT, Keys.LeftAlt },
        { (int)SDL.SDL_Keycode.SDLK_RALT, Keys.RightAlt },
        { (int)SDL.SDL_Keycode.SDLK_LCTRL, Keys.LeftControl },
        { (int)SDL.SDL_Keycode.SDLK_RCTRL, Keys.RightControl },
        { (int)SDL.SDL_Keycode.SDLK_LGUI, Keys.LeftWindows },
        { (int)SDL.SDL_Keycode.SDLK_RGUI, Keys.RightWindows },
        { (int)SDL.SDL_Keycode.SDLK_LSHIFT, Keys.LeftShift },
        { (int)SDL.SDL_Keycode.SDLK_RSHIFT, Keys.RightShift },
        { (int)SDL.SDL_Keycode.SDLK_APPLICATION, Keys.Apps },
        { (int)SDL.SDL_Keycode.SDLK_MENU, Keys.Apps },
        { (int)SDL.SDL_Keycode.SDLK_SLASH, Keys.OemQuestion },
        { (int)SDL.SDL_Keycode.SDLK_BACKSLASH, Keys.OemPipe },
        { (int)SDL.SDL_Keycode.SDLK_LEFTBRACKET, Keys.OemOpenBrackets },
        { (int)SDL.SDL_Keycode.SDLK_RIGHTBRACKET, Keys.OemCloseBrackets },
        { (int)SDL.SDL_Keycode.SDLK_CAPSLOCK, Keys.CapsLock },
        { (int)SDL.SDL_Keycode.SDLK_COMMA, Keys.OemComma },
        { (int)SDL.SDL_Keycode.SDLK_DELETE, Keys.Delete },
        { (int)SDL.SDL_Keycode.SDLK_END, Keys.End },
        { (int)SDL.SDL_Keycode.SDLK_BACKSPACE, Keys.Back },
        { (int)SDL.SDL_Keycode.SDLK_RETURN, Keys.Enter },
        { (int)SDL.SDL_Keycode.SDLK_ESCAPE, Keys.Escape },
        { (int)SDL.SDL_Keycode.SDLK_HOME, Keys.Home },
        { (int)SDL.SDL_Keycode.SDLK_INSERT, Keys.Insert },
        { (int)SDL.SDL_Keycode.SDLK_MINUS, Keys.OemMinus },
        { (int)SDL.SDL_Keycode.SDLK_NUMLOCKCLEAR, Keys.NumLock },
        { (int)SDL.SDL_Keycode.SDLK_PAGEUP, Keys.PageUp },
        { (int)SDL.SDL_Keycode.SDLK_PAGEDOWN, Keys.PageDown },
        { (int)SDL.SDL_Keycode.SDLK_PAUSE, Keys.Pause },
        { (int)SDL.SDL_Keycode.SDLK_PERIOD, Keys.OemPeriod },
        { (int)SDL.SDL_Keycode.SDLK_EQUALS, Keys.OemPlus },
        { (int)SDL.SDL_Keycode.SDLK_PRINTSCREEN, Keys.PrintScreen },
        { (int)SDL.SDL_Keycode.SDLK_APOSTROPHE, Keys.OemQuotes },
        { (int)SDL.SDL_Keycode.SDLK_SCROLLLOCK, Keys.Scroll },
        { (int)SDL.SDL_Keycode.SDLK_SEMICOLON, Keys.OemSemicolon },
        { (int)SDL.SDL_Keycode.SDLK_SLEEP, Keys.Sleep },
        { (int)SDL.SDL_Keycode.SDLK_TAB, Keys.Tab },
        { (int)SDL.SDL_Keycode.SDLK_GRAVE, Keys.OemTilde },
        { (int)SDL.SDL_Keycode.SDLK_VOLUMEUP, Keys.VolumeUp },
        { (int)SDL.SDL_Keycode.SDLK_VOLUMEDOWN, Keys.VolumeDown },
        { '²' /* FIXME: AZERTY SDL3? -flibit */, Keys.OemTilde },
        { 'é' /* FIXME: BEPO SDL3? -flibit */, Keys.None },
        { '|' /* FIXME: Norwegian SDL3? -flibit */, Keys.OemPipe },
        { '+' /* FIXME: Norwegian SDL3? -flibit */, Keys.OemPlus },
        { 'ø' /* FIXME: Norwegian SDL3? -flibit */, Keys.OemSemicolon },
        { 'æ' /* FIXME: Norwegian SDL3? -flibit */, Keys.OemQuotes },
        { (int)SDL.SDL_Keycode.SDLK_UNKNOWN, Keys.None }
    };

    private static Dictionary<int, Keys> ScancodesToKeys = new()
    {
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_A, Keys.A },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_B, Keys.B },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_C, Keys.C },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_D, Keys.D },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_E, Keys.E },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F, Keys.F },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_G, Keys.G },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_H, Keys.H },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_I, Keys.I },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_J, Keys.J },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_K, Keys.K },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_L, Keys.L },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_M, Keys.M },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_N, Keys.N },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_O, Keys.O },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_P, Keys.P },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_Q, Keys.Q },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_R, Keys.R },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_S, Keys.S },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_T, Keys.T },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_U, Keys.U },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_V, Keys.V },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_W, Keys.W },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_X, Keys.X },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_Y, Keys.Y },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_Z, Keys.Z },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_0, Keys.D0 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_1, Keys.D1 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_2, Keys.D2 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_3, Keys.D3 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_4, Keys.D4 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_5, Keys.D5 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_6, Keys.D6 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_7, Keys.D7 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_8, Keys.D8 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_9, Keys.D9 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_0, Keys.NumPad0 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_1, Keys.NumPad1 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_2, Keys.NumPad2 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_3, Keys.NumPad3 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_4, Keys.NumPad4 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_5, Keys.NumPad5 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_6, Keys.NumPad6 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_7, Keys.NumPad7 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_8, Keys.NumPad8 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_9, Keys.NumPad9 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_CLEAR, Keys.OemClear },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_DECIMAL, Keys.Decimal },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_DIVIDE, Keys.Divide },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_ENTER, Keys.Enter },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_MINUS, Keys.Subtract },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY, Keys.Multiply },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_PERIOD, Keys.OemPeriod },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUS, Keys.Add },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F1, Keys.F1 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F2, Keys.F2 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F3, Keys.F3 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F4, Keys.F4 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F5, Keys.F5 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F6, Keys.F6 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F7, Keys.F7 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F8, Keys.F8 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F9, Keys.F9 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F10, Keys.F10 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F11, Keys.F11 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F12, Keys.F12 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F13, Keys.F13 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F14, Keys.F14 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F15, Keys.F15 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F16, Keys.F16 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F17, Keys.F17 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F18, Keys.F18 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F19, Keys.F19 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F20, Keys.F20 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F21, Keys.F21 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F22, Keys.F22 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F23, Keys.F23 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_F24, Keys.F24 },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE, Keys.Space },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_UP, Keys.Up },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN, Keys.Down },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT, Keys.Left },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT, Keys.Right },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_LALT, Keys.LeftAlt },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RALT, Keys.RightAlt },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_LCTRL, Keys.LeftControl },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RCTRL, Keys.RightControl },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_LGUI, Keys.LeftWindows },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RGUI, Keys.RightWindows },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_LSHIFT, Keys.LeftShift },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RSHIFT, Keys.RightShift },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_APPLICATION, Keys.Apps },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_MENU, Keys.Apps },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_SLASH, Keys.OemQuestion },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_BACKSLASH, Keys.OemPipe },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_LEFTBRACKET, Keys.OemOpenBrackets },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET, Keys.OemCloseBrackets },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_CAPSLOCK, Keys.CapsLock },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_COMMA, Keys.OemComma },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_DELETE, Keys.Delete },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_END, Keys.End },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE, Keys.Back },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN, Keys.Enter },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE, Keys.Escape },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_HOME, Keys.Home },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_INSERT, Keys.Insert },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_MINUS, Keys.OemMinus },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR, Keys.NumLock },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP, Keys.PageUp },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN, Keys.PageDown },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_PAUSE, Keys.Pause },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_PERIOD, Keys.OemPeriod },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_EQUALS, Keys.OemPlus },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_PRINTSCREEN, Keys.PrintScreen },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_APOSTROPHE, Keys.OemQuotes },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_SCROLLLOCK, Keys.Scroll },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_SEMICOLON, Keys.OemSemicolon },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_SLEEP, Keys.Sleep },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_TAB, Keys.Tab },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_GRAVE, Keys.OemTilde },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEUP, Keys.VolumeUp },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEDOWN, Keys.VolumeDown },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN, Keys.None },
        /* FIXME: The following scancodes need verification! */
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_NONUSHASH, Keys.None },
        { (int)SDL.SDL_Scancode.SDL_SCANCODE_NONUSBACKSLASH, Keys.None }
    };

    private static Dictionary<int, SDL.SDL_Scancode> KeysToScancodes = new()
    {
        { (int)Keys.A, SDL.SDL_Scancode.SDL_SCANCODE_A },
        { (int)Keys.B, SDL.SDL_Scancode.SDL_SCANCODE_B },
        { (int)Keys.C, SDL.SDL_Scancode.SDL_SCANCODE_C },
        { (int)Keys.D, SDL.SDL_Scancode.SDL_SCANCODE_D },
        { (int)Keys.E, SDL.SDL_Scancode.SDL_SCANCODE_E },
        { (int)Keys.F, SDL.SDL_Scancode.SDL_SCANCODE_F },
        { (int)Keys.G, SDL.SDL_Scancode.SDL_SCANCODE_G },
        { (int)Keys.H, SDL.SDL_Scancode.SDL_SCANCODE_H },
        { (int)Keys.I, SDL.SDL_Scancode.SDL_SCANCODE_I },
        { (int)Keys.J, SDL.SDL_Scancode.SDL_SCANCODE_J },
        { (int)Keys.K, SDL.SDL_Scancode.SDL_SCANCODE_K },
        { (int)Keys.L, SDL.SDL_Scancode.SDL_SCANCODE_L },
        { (int)Keys.M, SDL.SDL_Scancode.SDL_SCANCODE_M },
        { (int)Keys.N, SDL.SDL_Scancode.SDL_SCANCODE_N },
        { (int)Keys.O, SDL.SDL_Scancode.SDL_SCANCODE_O },
        { (int)Keys.P, SDL.SDL_Scancode.SDL_SCANCODE_P },
        { (int)Keys.Q, SDL.SDL_Scancode.SDL_SCANCODE_Q },
        { (int)Keys.R, SDL.SDL_Scancode.SDL_SCANCODE_R },
        { (int)Keys.S, SDL.SDL_Scancode.SDL_SCANCODE_S },
        { (int)Keys.T, SDL.SDL_Scancode.SDL_SCANCODE_T },
        { (int)Keys.U, SDL.SDL_Scancode.SDL_SCANCODE_U },
        { (int)Keys.V, SDL.SDL_Scancode.SDL_SCANCODE_V },
        { (int)Keys.W, SDL.SDL_Scancode.SDL_SCANCODE_W },
        { (int)Keys.X, SDL.SDL_Scancode.SDL_SCANCODE_X },
        { (int)Keys.Y, SDL.SDL_Scancode.SDL_SCANCODE_Y },
        { (int)Keys.Z, SDL.SDL_Scancode.SDL_SCANCODE_Z },
        { (int)Keys.D0, SDL.SDL_Scancode.SDL_SCANCODE_0 },
        { (int)Keys.D1, SDL.SDL_Scancode.SDL_SCANCODE_1 },
        { (int)Keys.D2, SDL.SDL_Scancode.SDL_SCANCODE_2 },
        { (int)Keys.D3, SDL.SDL_Scancode.SDL_SCANCODE_3 },
        { (int)Keys.D4, SDL.SDL_Scancode.SDL_SCANCODE_4 },
        { (int)Keys.D5, SDL.SDL_Scancode.SDL_SCANCODE_5 },
        { (int)Keys.D6, SDL.SDL_Scancode.SDL_SCANCODE_6 },
        { (int)Keys.D7, SDL.SDL_Scancode.SDL_SCANCODE_7 },
        { (int)Keys.D8, SDL.SDL_Scancode.SDL_SCANCODE_8 },
        { (int)Keys.D9, SDL.SDL_Scancode.SDL_SCANCODE_9 },
        { (int)Keys.NumPad0, SDL.SDL_Scancode.SDL_SCANCODE_KP_0 },
        { (int)Keys.NumPad1, SDL.SDL_Scancode.SDL_SCANCODE_KP_1 },
        { (int)Keys.NumPad2, SDL.SDL_Scancode.SDL_SCANCODE_KP_2 },
        { (int)Keys.NumPad3, SDL.SDL_Scancode.SDL_SCANCODE_KP_3 },
        { (int)Keys.NumPad4, SDL.SDL_Scancode.SDL_SCANCODE_KP_4 },
        { (int)Keys.NumPad5, SDL.SDL_Scancode.SDL_SCANCODE_KP_5 },
        { (int)Keys.NumPad6, SDL.SDL_Scancode.SDL_SCANCODE_KP_6 },
        { (int)Keys.NumPad7, SDL.SDL_Scancode.SDL_SCANCODE_KP_7 },
        { (int)Keys.NumPad8, SDL.SDL_Scancode.SDL_SCANCODE_KP_8 },
        { (int)Keys.NumPad9, SDL.SDL_Scancode.SDL_SCANCODE_KP_9 },
        { (int)Keys.OemClear, SDL.SDL_Scancode.SDL_SCANCODE_KP_CLEAR },
        { (int)Keys.Decimal, SDL.SDL_Scancode.SDL_SCANCODE_KP_DECIMAL },
        { (int)Keys.Divide, SDL.SDL_Scancode.SDL_SCANCODE_KP_DIVIDE },
        { (int)Keys.Multiply, SDL.SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY },
        { (int)Keys.Subtract, SDL.SDL_Scancode.SDL_SCANCODE_KP_MINUS },
        { (int)Keys.Add, SDL.SDL_Scancode.SDL_SCANCODE_KP_PLUS },
        { (int)Keys.F1, SDL.SDL_Scancode.SDL_SCANCODE_F1 },
        { (int)Keys.F2, SDL.SDL_Scancode.SDL_SCANCODE_F2 },
        { (int)Keys.F3, SDL.SDL_Scancode.SDL_SCANCODE_F3 },
        { (int)Keys.F4, SDL.SDL_Scancode.SDL_SCANCODE_F4 },
        { (int)Keys.F5, SDL.SDL_Scancode.SDL_SCANCODE_F5 },
        { (int)Keys.F6, SDL.SDL_Scancode.SDL_SCANCODE_F6 },
        { (int)Keys.F7, SDL.SDL_Scancode.SDL_SCANCODE_F7 },
        { (int)Keys.F8, SDL.SDL_Scancode.SDL_SCANCODE_F8 },
        { (int)Keys.F9, SDL.SDL_Scancode.SDL_SCANCODE_F9 },
        { (int)Keys.F10, SDL.SDL_Scancode.SDL_SCANCODE_F10 },
        { (int)Keys.F11, SDL.SDL_Scancode.SDL_SCANCODE_F11 },
        { (int)Keys.F12, SDL.SDL_Scancode.SDL_SCANCODE_F12 },
        { (int)Keys.F13, SDL.SDL_Scancode.SDL_SCANCODE_F13 },
        { (int)Keys.F14, SDL.SDL_Scancode.SDL_SCANCODE_F14 },
        { (int)Keys.F15, SDL.SDL_Scancode.SDL_SCANCODE_F15 },
        { (int)Keys.F16, SDL.SDL_Scancode.SDL_SCANCODE_F16 },
        { (int)Keys.F17, SDL.SDL_Scancode.SDL_SCANCODE_F17 },
        { (int)Keys.F18, SDL.SDL_Scancode.SDL_SCANCODE_F18 },
        { (int)Keys.F19, SDL.SDL_Scancode.SDL_SCANCODE_F19 },
        { (int)Keys.F20, SDL.SDL_Scancode.SDL_SCANCODE_F20 },
        { (int)Keys.F21, SDL.SDL_Scancode.SDL_SCANCODE_F21 },
        { (int)Keys.F22, SDL.SDL_Scancode.SDL_SCANCODE_F22 },
        { (int)Keys.F23, SDL.SDL_Scancode.SDL_SCANCODE_F23 },
        { (int)Keys.F24, SDL.SDL_Scancode.SDL_SCANCODE_F24 },
        { (int)Keys.Space, SDL.SDL_Scancode.SDL_SCANCODE_SPACE },
        { (int)Keys.Up, SDL.SDL_Scancode.SDL_SCANCODE_UP },
        { (int)Keys.Down, SDL.SDL_Scancode.SDL_SCANCODE_DOWN },
        { (int)Keys.Left, SDL.SDL_Scancode.SDL_SCANCODE_LEFT },
        { (int)Keys.Right, SDL.SDL_Scancode.SDL_SCANCODE_RIGHT },
        { (int)Keys.LeftAlt, SDL.SDL_Scancode.SDL_SCANCODE_LALT },
        { (int)Keys.RightAlt, SDL.SDL_Scancode.SDL_SCANCODE_RALT },
        { (int)Keys.LeftControl, SDL.SDL_Scancode.SDL_SCANCODE_LCTRL },
        { (int)Keys.RightControl, SDL.SDL_Scancode.SDL_SCANCODE_RCTRL },
        { (int)Keys.LeftWindows, SDL.SDL_Scancode.SDL_SCANCODE_LGUI },
        { (int)Keys.RightWindows, SDL.SDL_Scancode.SDL_SCANCODE_RGUI },
        { (int)Keys.LeftShift, SDL.SDL_Scancode.SDL_SCANCODE_LSHIFT },
        { (int)Keys.RightShift, SDL.SDL_Scancode.SDL_SCANCODE_RSHIFT },
        { (int)Keys.Apps, SDL.SDL_Scancode.SDL_SCANCODE_APPLICATION },
        { (int)Keys.OemQuestion, SDL.SDL_Scancode.SDL_SCANCODE_SLASH },
        { (int)Keys.OemPipe, SDL.SDL_Scancode.SDL_SCANCODE_BACKSLASH },
        { (int)Keys.OemOpenBrackets, SDL.SDL_Scancode.SDL_SCANCODE_LEFTBRACKET },
        { (int)Keys.OemCloseBrackets, SDL.SDL_Scancode.SDL_SCANCODE_RIGHTBRACKET },
        { (int)Keys.CapsLock, SDL.SDL_Scancode.SDL_SCANCODE_CAPSLOCK },
        { (int)Keys.OemComma, SDL.SDL_Scancode.SDL_SCANCODE_COMMA },
        { (int)Keys.Delete, SDL.SDL_Scancode.SDL_SCANCODE_DELETE },
        { (int)Keys.End, SDL.SDL_Scancode.SDL_SCANCODE_END },
        { (int)Keys.Back, SDL.SDL_Scancode.SDL_SCANCODE_BACKSPACE },
        { (int)Keys.Enter, SDL.SDL_Scancode.SDL_SCANCODE_RETURN },
        { (int)Keys.Escape, SDL.SDL_Scancode.SDL_SCANCODE_ESCAPE },
        { (int)Keys.Home, SDL.SDL_Scancode.SDL_SCANCODE_HOME },
        { (int)Keys.Insert, SDL.SDL_Scancode.SDL_SCANCODE_INSERT },
        { (int)Keys.OemMinus, SDL.SDL_Scancode.SDL_SCANCODE_MINUS },
        { (int)Keys.NumLock, SDL.SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR },
        { (int)Keys.PageUp, SDL.SDL_Scancode.SDL_SCANCODE_PAGEUP },
        { (int)Keys.PageDown, SDL.SDL_Scancode.SDL_SCANCODE_PAGEDOWN },
        { (int)Keys.Pause, SDL.SDL_Scancode.SDL_SCANCODE_PAUSE },
        { (int)Keys.OemPeriod, SDL.SDL_Scancode.SDL_SCANCODE_PERIOD },
        { (int)Keys.OemPlus, SDL.SDL_Scancode.SDL_SCANCODE_EQUALS },
        { (int)Keys.PrintScreen, SDL.SDL_Scancode.SDL_SCANCODE_PRINTSCREEN },
        { (int)Keys.OemQuotes, SDL.SDL_Scancode.SDL_SCANCODE_APOSTROPHE },
        { (int)Keys.Scroll, SDL.SDL_Scancode.SDL_SCANCODE_SCROLLLOCK },
        { (int)Keys.OemSemicolon, SDL.SDL_Scancode.SDL_SCANCODE_SEMICOLON },
        { (int)Keys.Sleep, SDL.SDL_Scancode.SDL_SCANCODE_SLEEP },
        { (int)Keys.Tab, SDL.SDL_Scancode.SDL_SCANCODE_TAB },
        { (int)Keys.OemTilde, SDL.SDL_Scancode.SDL_SCANCODE_GRAVE },
        { (int)Keys.VolumeUp, SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEUP },
        { (int)Keys.VolumeDown, SDL.SDL_Scancode.SDL_SCANCODE_VOLUMEDOWN },
        { (int)Keys.None, SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN }
    };

    public IntPtr CreateWindow(Size size)
    {
        const SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_HIDDEN
                                      | SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;
        var wnd = SDL_CreateWindow("Window", size.Width, size.Height, flags);
        if (wnd == IntPtr.Zero)
        {
            throw new PlatformException($"Failed to create window: {SDL_GetError()}");
        }

        return wnd;
    }

    public void DestroyWindow(IntPtr window)
    {
        SDL_DestroyWindow(window);
    }

    public void SetWindowTitle(IntPtr window, string title)
    {
        Debug.Assert(window != IntPtr.Zero);
        SDL_SetWindowTitle(window, title);
    }

    public string GetWindowTitle(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return string.Empty;
        return SDL_GetWindowTitle(window);
    }

    public void SetWindowSize(IntPtr window, Size size)
    {
        Debug.Assert(window != IntPtr.Zero);
        SDL_SetWindowSize(window, size.Width, size.Height);
    }

    public Size GetWindowSize(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return Size.Empty;
        if (!SDL_GetWindowSize(window, out var w, out var h))
            return Size.Empty;
        return new Size(w, h);
    }

    public Size GetWindowPixelSize(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return Size.Empty;

        SDL_GetWindowSizeInPixels(window, out var w, out var h);
        return new Size(w, h);
    }

    public void ShowWindow(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return;

        SDL_ShowWindow(window);
        SDL_RaiseWindow(window);
    }

    public void HideWindow(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return;

        SDL_HideWindow(window);
    }

    public uint GetWindowDisplayIndex(IntPtr window)
    {
        if (window == IntPtr.Zero)
            return uint.MaxValue;
        
        return SDL_GetDisplayForWindow(window);
    }

    public string GetBasePath()
    {
        return SDL_GetBasePath();
    }

    public string GetWritePath()
    {
        var creator = SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_CREATOR_STRING);
        var name = SDL_GetAppMetadataProperty(SDL_PROP_APP_METADATA_NAME_STRING);
        return SDL_GetPrefPath(creator.ReplaceAll('_', Path.GetInvalidPathChars()),
            name.ReplaceAll('_', Path.GetInvalidPathChars()));
    }

    public double GetPerformanceCounter()
    {
        return SDL_GetPerformanceCounter();
    }

    public double GetPerformanceCounterFrequency()
    {
        return SDL_GetPerformanceFrequency();
    }
}