using System.Drawing;
using JetBrains.Annotations;
using Radish.Input;

namespace Radish.Platform;

[PublicAPI]
public interface IPlatformBackend : IDisposable
{
    string Name { get; }

    #region Events
    void PumpEvents();
    bool WantsToQuit { get; }
    #endregion

    #region Windows
    IntPtr CreateWindow(Size size);
    void DestroyWindow(IntPtr window);
    void SetWindowTitle(IntPtr window, string title);
    string GetWindowTitle(IntPtr window);
    void SetWindowSize(IntPtr window, Size size);
    Size GetWindowSize(IntPtr window);
    Size GetWindowPixelSize(IntPtr window);
    void ShowWindow(IntPtr window);
    void HideWindow(IntPtr window);
    uint GetWindowDisplayIndex(IntPtr window);
    #endregion

    #region Paths
    string GetBasePath();
    string GetWritePath();
    #endregion

    #region Graphics
    IPlatformRenderer GetRenderBackend();
    void Initialized();
    #endregion

    #region Timers

    double GetPerformanceCounter();
    double GetPerformanceCounterFrequency();

    #endregion

    #region Gamepad

    GamepadType GetGamepadType(uint id);
    string? GetGamepadName(uint id);
    void SetGamepadPlayerIndex(uint id, int index);
    int GetGamepadPlayerIndex(uint id);
    void SetGamepadLED(uint id, Color color);
    void SendGamepadRumblePacket(uint id, in GamepadRumbleState state);
    string GetGamepadSerial(uint id);

    #endregion
    
    #region Keyboard

    Keys ScancodeToKeycode(Keys scancode);
    string GetKeyboardName(uint id);

    #endregion
    
    #region Mice

    string GetMouseName(uint id);

    #endregion

    #region Text Input

    void BeginTextInput();
    void EndTextInput();
    void SetTextInputRect(Rectangle rect);

    #endregion
}