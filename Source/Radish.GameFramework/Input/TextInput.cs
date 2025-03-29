using System.Drawing;
using JetBrains.Annotations;
using Radish.Platform;

namespace Radish.Input;

[PublicAPI]
public static class TextInput
{
    public delegate void InputReceivedDelegate(char c);
    
    public static event InputReceivedDelegate OnTextInput
    {
        add => _onTextInput += value;
        remove
        {
            if (_onTextInput != null)
                _onTextInput -= value;
        }
    }

    private static InputReceivedDelegate? _onTextInput;
    private static IPlatformBackend _platform = null!;
    
    internal static void SetPlatformBackend(IPlatformBackend platform)
    {
        _platform = platform;
    }

    public static void BeginTextInput()
    {
        _platform.BeginTextInput();
    }

    public static void EndTextInput()
    {
        _platform.EndTextInput();
    }

    public static void SetTextInputRect(Rectangle rect)
    {
        _platform.SetTextInputRect(rect);
    }

    internal static void NotifyTextInput(char c)
    {
        _onTextInput?.Invoke(c);
    }
}