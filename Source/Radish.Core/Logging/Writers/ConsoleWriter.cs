namespace Radish.Logging.Writers;

public sealed class ConsoleWriter : ILogWriter
{
    public void Write(LogLevel level, string category, in ReadOnlySpan<char> formattedMessage)
    {
#if !DEBUG
        if (level == LogLevel.Debug)
            return;
#endif
        
        var location = level switch
        {
            LogLevel.Info or LogLevel.Debug => Console.Out,
            _ => Console.Error
        };

        var levelString = level switch
        {
            LogLevel.Info => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "EROR",
            LogLevel.Exception => "EXPT",
            LogLevel.Debug => "DEBG",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
        
        location.Write($"[{levelString}] [{category}] ");
        location.WriteLine(formattedMessage);
    }
}