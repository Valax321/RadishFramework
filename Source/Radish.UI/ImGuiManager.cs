using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using ImGuiNET;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Radish.Framework;
using Radish.Graphics;
using Radish.Input;
using Radish.Platform;
using Radish.Services;

namespace Radish.UI;

[PublicAPI]
public unsafe sealed class ImGuiManager : 
    IDisposable, 
    IGameDraw, 
    IGameUpdate,
    IServiceConsumer
{
    // Needs to update FIRST and draw LAST
    public int DrawOrder => int.MaxValue;
    public int UpdateOrder => int.MinValue;

    private IntPtr _context;
    private IGraphicsDevice _graphicsDevice = null!;
    private IPlatformBackend _backend = null!;

    private KeyboardState _lastKeyboardState;
    private MouseState _lastMouseState;
    private GamepadState _lastGamepadState;
    private ButtonState[] mouseClicks = new ButtonState[5];

    public ImGuiManager(GameServiceCollection services)
    {
        services.AddSingleton(this);
        
        _context = ImGui.CreateContext();
        ConfigureImGui();
    }

    private void ConfigureImGui()
    {
        var io = ImGui.GetIO();
        io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors | 
                           ImGuiBackendFlags.HasGamepad | 
                           ImGuiBackendFlags.HasSetMousePos;
        
        if (OperatingSystem.IsMacOS())
            io.ConfigMacOSXBehaviors = true;

        io.NativePtr->BackendPlatformName = Utf8StringMarshaller.ConvertToUnmanaged("RadishFramework");
        io.NativePtr->BackendRendererName = Utf8StringMarshaller.ConvertToUnmanaged("RadishFramework");
        
        TextInput.OnTextInput += OnTextInput;
        Mouse.OnClick += OnMouseClick;
    }

    private void OnMouseClick(uint id, MouseButton button, int clickcount)
    {
        mouseClicks[(int)button] = ButtonState.Down;
    }

    private void OnTextInput(char c)
    {
        var io = ImGui.GetIO();
        io.AddInputCharacterUTF16(c);
    }

    public void ResolveServices(IServiceProvider services)
    {
        _graphicsDevice = services.GetRequiredService<IGraphicsDevice>();
        _backend = services.GetRequiredService<IPlatformBackend>();
    }
    
    public void Update(TimeSpan deltaTime)
    {
        var io = ImGui.GetIO();
        io.DeltaTime = (float)deltaTime.TotalSeconds;
        io.DisplaySize = new Vector2(_graphicsDevice.BackbufferSize.Width, _graphicsDevice.BackbufferSize.Height);

        var scale = _graphicsDevice.DisplayScale;
        io.DisplayFramebufferScale = new Vector2(scale, scale);
        
        UpdateKeyboardState();
        UpdateMouseState();
        UpdateGamepadState();
        
        ImGui.NewFrame();
    }

    private void UpdateKeyboardState()
    {
        var io = ImGui.GetIO();
        if (io.WantTextInput)
        {
            TextInput.BeginTextInput();
        }
        else
        {
            TextInput.EndTextInput();
        }

        if (Keyboard.CurrentActiveKeyboardIndex < 0)
        {
            _lastKeyboardState = new KeyboardState();
            return;
        }
        
        var state = Keyboard.GetState((uint)Keyboard.CurrentActiveKeyboardIndex);

        foreach (var k in Enum.GetValues<Keys>())
        {
            if (k == Keys.None)
                continue;
            var keycode = Keyboard.ScancodeToKeycode(k);
            if (keycode == Keys.None)
                continue;

            var justDown = state.IsKeyDown(keycode) && _lastKeyboardState.IsKeyUp(keycode);
            var justUp = state.IsKeyUp(keycode) && _lastKeyboardState.IsKeyDown(keycode);
            if (!(justDown || justUp))
                continue;
            
            switch (keycode)
            {
                case Keys.LeftAlt:
                case Keys.RightAlt:
                    io.AddKeyEvent(ImGuiKey.ModAlt, justDown);
                    break;
                case Keys.LeftShift:
                case Keys.RightShift:
                    io.AddKeyEvent(ImGuiKey.ModShift, justDown);
                    break;
                case Keys.LeftControl:
                case Keys.RightControl:
                    io.AddKeyEvent(ImGuiKey.ModCtrl, justDown);
                    break;
                case Keys.LeftWindows:
                case Keys.RightWindows:
                    io.AddKeyEvent(ImGuiKey.ModSuper, justDown);
                    break;
            }
            
            var ik = RadishKeyToImGuiKey(keycode);
            if (ik == ImGuiKey.NamedKey_COUNT) // We couldn't convert this key to imgui format
                continue;
            
            io.AddKeyEvent(ik, justDown);
            io.SetKeyEventNativeData(ik, (int)keycode, (int)k);
        }
        
        _lastKeyboardState = state;
    }

    private void UpdateMouseState()
    {
        var io = ImGui.GetIO();
        if (io.WantSetMousePos)
        {
            Mouse.SetPosition((int)io.MousePos.X, (int)io.MousePos.Y);
        }
        
        if (Mouse.CurrentActiveMouseIndex < 0)
        {
            _lastMouseState = new MouseState();
            return;
        }
        
        var state = Mouse.GetState((uint)Mouse.CurrentActiveMouseIndex);
        
        io.AddMouseSourceEvent(ImGuiMouseSource.Mouse);

        for (var i = 0; i < mouseClicks.Length; ++i)
        {
            var s = state[(MouseButton)i] | mouseClicks[i];
            if (s == ButtonState.Up)
                io.AddMouseButtonEvent(i, false);
            else if (_lastMouseState[(MouseButton)i] == ButtonState.Up)
                io.AddMouseButtonEvent(i, true);
            mouseClicks[i] = ButtonState.Up;
        }
        
        io.AddMousePosEvent(state.Position.X * io.DisplayFramebufferScale.X, state.Position.Y * io.DisplayFramebufferScale.Y);
        io.AddMouseWheelEvent(0, (state.Wheel.Y - _lastMouseState.Wheel.Y) / 100.0f);

        _lastMouseState = state;

        if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
            return;
        /*
         TODO: move to backend
        var cursor = ImGui.GetMouseCursor();
        if (io.MouseDrawCursor || cursor == ImGuiMouseCursor.None)
            SDL.SDL_HideCursor();
        else
        {
            var c = _cursors.GetValueOrDefault(cursor);
            if (c == IntPtr.Zero)
                c = _cursors[ImGuiMouseCursor.Arrow];
            SDL.SDL_SetCursor(c);
            SDL.SDL_ShowCursor();
        }
        */
    }
    
    private void UpdateGamepadState()
    {
        if (Gamepad.CurrentActiveGamepadIndex < 0)
        {
            _lastGamepadState = new GamepadState();
        }
        
        var io = ImGui.GetIO();
        var state = Gamepad.GetState((uint)Gamepad.CurrentActiveGamepadIndex);
        foreach (var b in Enum.GetValues<GamepadButton>())
        {
            var justDown = state.IsButtonDown(b) && _lastGamepadState.IsButtonUp(b);
            var justUp = state.IsButtonUp(b) && _lastGamepadState.IsButtonDown(b);
            if (!(justDown || justUp))
                continue;

            var k = b switch
            {
                GamepadButton.DPadUp => ImGuiKey.GamepadDpadUp,
                GamepadButton.DPadDown => ImGuiKey.GamepadDpadDown,
                GamepadButton.DPadLeft => ImGuiKey.GamepadDpadLeft,
                GamepadButton.DPadRight => ImGuiKey.GamepadDpadRight,
                GamepadButton.Options => ImGuiKey.GamepadStart,
                GamepadButton.Back => ImGuiKey.GamepadBack,
                GamepadButton.LeftStickPress => ImGuiKey.GamepadL3,
                GamepadButton.RightStickPress => ImGuiKey.GamepadR3,
                GamepadButton.LeftShoulder => ImGuiKey.GamepadL1,
                GamepadButton.RightShoulder => ImGuiKey.GamepadR1,
                GamepadButton.FaceSouth => ImGuiKey.GamepadFaceDown,
                GamepadButton.FaceEast => ImGuiKey.GamepadFaceRight,
                GamepadButton.FaceWest => ImGuiKey.GamepadFaceLeft,
                GamepadButton.FaceNorth => ImGuiKey.GamepadFaceUp,
                _ => ImGuiKey.NamedKey_COUNT
            };

            if (k == ImGuiKey.NamedKey_COUNT)
                continue;
            
            io.AddKeyEvent(k, justDown);
        }
        SetAnalogKey(ImGuiKey.GamepadL2, state.GetAxis(GamepadAxis.LeftTrigger));
        SetAnalogKey(ImGuiKey.GamepadR2, state.GetAxis(GamepadAxis.RightTrigger));
        SetAnalogKey(ImGuiKey.GamepadLStickUp, state.GetAxis(GamepadAxis.LeftStickY));
        SetAnalogKey(ImGuiKey.GamepadLStickDown, -state.GetAxis(GamepadAxis.LeftStickY));
        SetAnalogKey(ImGuiKey.GamepadLStickRight, state.GetAxis(GamepadAxis.LeftStickX));
        SetAnalogKey(ImGuiKey.GamepadLStickRight, -state.GetAxis(GamepadAxis.LeftStickX));
        SetAnalogKey(ImGuiKey.GamepadRStickUp, state.GetAxis(GamepadAxis.RightStickY));
        SetAnalogKey(ImGuiKey.GamepadRStickDown, -state.GetAxis(GamepadAxis.RightStickY));
        SetAnalogKey(ImGuiKey.GamepadRStickRight, state.GetAxis(GamepadAxis.RightStickX));
        SetAnalogKey(ImGuiKey.GamepadRStickRight, -state.GetAxis(GamepadAxis.RightStickX));
        
        _lastGamepadState = state;
        return;

        void SetAnalogKey(ImGuiKey key, float value)
        {
            io.AddKeyAnalogEvent(key, value > 0.1f, value > 0.0f ? value : 0.0f);
        }
    }
    
    public void Draw(TimeSpan deltaTime)
    {
        ImGui.Render();
    }
    
    public void Dispose()
    {
        ImGui.DestroyContext(_context);
        Mouse.OnClick -= OnMouseClick;
        TextInput.OnTextInput -= OnTextInput;
        _context = IntPtr.Zero;
    }
    
      private static ImGuiKey RadishKeyToImGuiKey(Keys keys)
    {
        // all work and no play makes jack a dull boy
        return keys switch
        {
            Keys.Back => ImGuiKey.Backspace,
            Keys.Tab => ImGuiKey.Tab,
            Keys.Enter => ImGuiKey.Enter,
            Keys.CapsLock => ImGuiKey.CapsLock,
            Keys.Escape => ImGuiKey.Escape,
            Keys.Space => ImGuiKey.Space,
            Keys.PageUp => ImGuiKey.PageUp,
            Keys.PageDown => ImGuiKey.PageDown,
            Keys.End => ImGuiKey.End,
            Keys.Home => ImGuiKey.Home,
            Keys.Left => ImGuiKey.LeftArrow,
            Keys.Up => ImGuiKey.UpArrow,
            Keys.Right => ImGuiKey.RightArrow,
            Keys.Down => ImGuiKey.DownArrow,
            //Keys.Select => ImGuiKey.S,
            //Keys.Print => ImGuiKey.PrintScreen,
            //Keys.Execute => ImGuiKey.Execute,
            Keys.PrintScreen => ImGuiKey.PrintScreen,
            Keys.Insert => ImGuiKey.Insert,
            Keys.Delete => ImGuiKey.Delete,
            //Keys.Help => ImGuiKey.Help,
            Keys.D0 => ImGuiKey._0,
            Keys.D1 => ImGuiKey._1,
            Keys.D2 => ImGuiKey._2,
            Keys.D3 => ImGuiKey._3,
            Keys.D4 => ImGuiKey._4,
            Keys.D5 => ImGuiKey._5,
            Keys.D6 => ImGuiKey._6,
            Keys.D7 => ImGuiKey._7,
            Keys.D8 => ImGuiKey._8,
            Keys.D9 => ImGuiKey._9,
            Keys.A => ImGuiKey.A,
            Keys.B => ImGuiKey.B,
            Keys.C => ImGuiKey.C,
            Keys.D => ImGuiKey.D,
            Keys.E => ImGuiKey.E,
            Keys.F => ImGuiKey.F,
            Keys.G => ImGuiKey.G,
            Keys.H => ImGuiKey.H,
            Keys.I => ImGuiKey.I,
            Keys.J => ImGuiKey.J,
            Keys.K => ImGuiKey.K,
            Keys.L => ImGuiKey.L,
            Keys.M => ImGuiKey.M,
            Keys.N => ImGuiKey.N,
            Keys.O => ImGuiKey.O,
            Keys.P => ImGuiKey.P,
            Keys.Q => ImGuiKey.Q,
            Keys.R => ImGuiKey.R,
            Keys.S => ImGuiKey.S,
            Keys.T => ImGuiKey.T,
            Keys.U => ImGuiKey.U,
            Keys.V => ImGuiKey.V,
            Keys.W => ImGuiKey.W,
            Keys.X => ImGuiKey.X,
            Keys.Y => ImGuiKey.Y,
            Keys.Z => ImGuiKey.Z,
            Keys.LeftWindows => ImGuiKey.LeftSuper,
            Keys.RightWindows => ImGuiKey.RightSuper,
            //Keys.Apps => ImGuiKey.Apps,
            //Keys.Sleep => ImGuiKey.sleep,
            Keys.NumPad0 => ImGuiKey.Keypad0,
            Keys.NumPad1 => ImGuiKey.Keypad1,
            Keys.NumPad2 => ImGuiKey.Keypad2,
            Keys.NumPad3 => ImGuiKey.Keypad3,
            Keys.NumPad4 => ImGuiKey.Keypad4,
            Keys.NumPad5 => ImGuiKey.Keypad5,
            Keys.NumPad6 => ImGuiKey.Keypad6,
            Keys.NumPad7 => ImGuiKey.Keypad7,
            Keys.NumPad8 => ImGuiKey.Keypad8,
            Keys.NumPad9 => ImGuiKey.Keypad9,
            Keys.Multiply => ImGuiKey.KeypadMultiply,
            Keys.Add => ImGuiKey.KeypadAdd,
            //Keys.Separator => ImGuiKey.separator,
            Keys.Subtract => ImGuiKey.KeypadSubtract,
            Keys.Decimal => ImGuiKey.KeypadDecimal,
            Keys.Divide => ImGuiKey.KeypadDivide,
            Keys.F1 => ImGuiKey.F1,
            Keys.F2 => ImGuiKey.F2,
            Keys.F3 => ImGuiKey.F3,
            Keys.F4 => ImGuiKey.F4,
            Keys.F5 => ImGuiKey.F5,
            Keys.F6 => ImGuiKey.F6,
            Keys.F7 => ImGuiKey.F7,
            Keys.F8 => ImGuiKey.F8,
            Keys.F9 => ImGuiKey.F9,
            Keys.F10 => ImGuiKey.F10,
            Keys.F11 => ImGuiKey.F11,
            Keys.F12 => ImGuiKey.F12,
            Keys.F13 => ImGuiKey.F13,
            Keys.F14 => ImGuiKey.F14,
            Keys.F15 => ImGuiKey.F15,
            Keys.F16 => ImGuiKey.F16,
            Keys.F17 => ImGuiKey.F17,
            Keys.F18 => ImGuiKey.F18,
            Keys.F19 => ImGuiKey.F19,
            Keys.F20 => ImGuiKey.F20,
            Keys.F21 => ImGuiKey.F21,
            Keys.F22 => ImGuiKey.F22,
            Keys.F23 => ImGuiKey.F23,
            Keys.F24 => ImGuiKey.F24,
            Keys.NumLock => ImGuiKey.NumLock,
            Keys.Scroll => ImGuiKey.ScrollLock,
            Keys.LeftShift => ImGuiKey.LeftShift,
            Keys.RightShift => ImGuiKey.RightShift,
            Keys.LeftControl => ImGuiKey.LeftCtrl,
            Keys.RightControl => ImGuiKey.RightCtrl,
            Keys.LeftAlt => ImGuiKey.LeftAlt,
            Keys.RightAlt => ImGuiKey.RightAlt,
            //Keys.BrowserBack => ImGuiKey.,
            //Keys.BrowserForward => expr,
            //Keys.BrowserRefresh => expr,
            //Keys.BrowserStop => expr,
            //Keys.BrowserSearch => expr,
            //Keys.BrowserFavorites => IUm,
            //Keys.BrowserHome => ImGuiKey.Home,
            /*
            Keys.VolumeMute => ImGuiKey.,
            Keys.VolumeDown => expr,
            Keys.VolumeUp => expr,
            Keys.MediaNextTrack => expr,
            Keys.MediaPreviousTrack => expr,
            Keys.MediaStop => expr,
            Keys.MediaPlayPause => expr,
            Keys.LaunchMail => expr,
            Keys.SelectMedia => expr,
            Keys.LaunchApplication1 => expr,
            Keys.LaunchApplication2 => expr,
            */
            Keys.OemSemicolon => ImGuiKey.Semicolon,
            Keys.OemPlus => ImGuiKey.Equal,
            Keys.OemComma => ImGuiKey.Comma,
            Keys.OemMinus => ImGuiKey.Minus,
            Keys.OemPeriod => ImGuiKey.Period,
            Keys.OemQuestion => ImGuiKey.Slash,
            Keys.OemTilde => ImGuiKey.GraveAccent,
            Keys.OemOpenBrackets => ImGuiKey.LeftBracket,
            //Keys.OemPipe => ImGuiKey.,
            Keys.OemCloseBrackets => ImGuiKey.RightBracket,
            Keys.OemQuotes => ImGuiKey.Apostrophe,
            //Keys.Oem8 => expr,
            Keys.OemBackslash => ImGuiKey.Backslash,
            //Keys.ProcessKey => ImGuiKey.pro,
            //Keys.Attn => expr,
            //Keys.Crsel => expr,
            //Keys.Exsel => expr,
            //Keys.EraseEof => expr,
            //Keys.Play => expr,
            //Keys.Zoom => expr,
            //Keys.Pa1 => expr,
            //Keys.OemClear => expr,
            //Keys.ChatPadGreen => expr,
            //Keys.ChatPadOrange => expr,
            Keys.Pause => ImGuiKey.Pause,
            //Keys.ImeConvert => expr,
            //Keys.ImeNoConvert => expr,
            //Keys.Kana => ImGuiKey.ka,
            //Keys.Kanji => expr,
            //Keys.OemAuto => expr,
            //Keys.OemCopy => expr,
            //Keys.OemEnlW => expr,
            _ => ImGuiKey.NamedKey_COUNT
        };
    }
}
