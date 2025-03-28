using System.Diagnostics;

namespace Radish.Logging.Writers;

public sealed class DebugWriter : ILogWriter
{
    public void Write(LogLevel level, string category, in ReadOnlySpan<char> formattedMessage)
    {
        Debug.WriteLine($"[{category}] {formattedMessage}", level);
    }
}