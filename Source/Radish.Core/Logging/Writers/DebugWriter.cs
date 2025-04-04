﻿using System.Diagnostics;

namespace Radish.Logging.Writers;

public sealed class DebugWriter : ILogWriter
{
    public void Write(LogLevel level, string category, in ReadOnlySpan<char> formattedMessage)
    {
#if !DEBUG
        if (level == LogLevel.Debug)
            return;
#endif
        
        Debug.WriteLine("[{0}] {1}", category, formattedMessage.ToString());
    }
}