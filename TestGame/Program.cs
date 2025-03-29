using Radish.Framework;
using Radish.Platform;

namespace Radish.TestGame;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        var opts = new ApplicationOptions(SDL3Platform.Create);
        using var app = new TestApplication(in opts);
        app.Run();
    }
}