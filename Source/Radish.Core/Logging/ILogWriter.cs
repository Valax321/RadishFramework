using System;
using JetBrains.Annotations;

namespace Radish.Logging
{
    /// <summary>
    /// Location to which a log message can be written.
    /// </summary>
    [PublicAPI]
    public interface ILogWriter
    {
        void Write(LogLevel level, string category, in ReadOnlySpan<char> formattedMessage);
    }
}
