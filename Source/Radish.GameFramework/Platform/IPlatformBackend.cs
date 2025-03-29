using System.Drawing;
using JetBrains.Annotations;

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
    void ShowWindow(IntPtr window);
    void HideWindow(IntPtr window);
    #endregion

    #region Paths
    string GetBasePath();
    string GetWritePath();
    #endregion

    #region Graphics
    IPlatformRenderer GetRenderBackend();
    #endregion

    #region Timers

    double GetPerformanceCounter();
    double GetPerformanceCounterFrequency();

    #endregion
}