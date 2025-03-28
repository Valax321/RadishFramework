using System.Diagnostics;
using JetBrains.Annotations;
using Radish.Logging.Writers;

namespace Radish.Logging;

[PublicAPI]
public static class LogManager
{
    internal static List<ILogWriter> RegisteredLoggers { get; } =
    [
        new ConsoleWriter(),
        new DebugWriter()
    ];
    
    public static ILogger GetCurrentClassLogger()
    {
        var trace = new StackTrace(1);
        Debug.Assert(trace.FrameCount >= 1);

        var callerFrame = trace.GetFrame(0);
        var method = DiagnosticMethodInfo.Create(callerFrame!);
        var callerType = method!.DeclaringTypeName;
        return new RadishLogger(callerType);
    }

    public static ILogger GetLoggerForType<T>() => GetLoggerForType(typeof(T));

    public static ILogger GetLoggerForType(Type t)
    {
        return new RadishLogger(t.FullName ?? "[null]");
    }

    public static ILogger GetNamedLogger(string name)
    {
        return new RadishLogger(name);
    }

    public static void RegisterLogWriter(ILogWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        RegisteredLoggers.Add(writer);
    }

    public static void ClearBuiltinWriters()
    {
        RegisteredLoggers.Clear();
    }
}