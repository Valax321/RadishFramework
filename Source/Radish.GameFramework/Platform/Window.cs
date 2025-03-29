using System.Drawing;
using JetBrains.Annotations;

namespace Radish.Platform;

[PublicAPI]
public class Window : IDisposable
{
    public IntPtr Handle { get; private set; }

    public string Title
    {
        get => _backend.GetWindowTitle(Handle);
        set => _backend.SetWindowTitle(Handle, value);
    }

    public Size Size
    {
        get => _backend.GetWindowSize(Handle);
        set => _backend.SetWindowSize(Handle, value);
    }

    private readonly IPlatformBackend _backend;

    internal Window(IPlatformBackend backend, Size size)
    {
        _backend = backend;
        Handle = backend.CreateWindow(size);
    }

    public void Show()
    {
        _backend.ShowWindow(Handle);
    }

    public void Hide()
    {
        _backend.HideWindow(Handle);
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _backend.DestroyWindow(Handle);
        Handle = IntPtr.Zero;
    }
}