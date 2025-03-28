namespace Radish.Logging.Writers;

public sealed class ConsoleWriter : ILogWriter
{
    public void Write(LogLevel level, string category, in ReadOnlySpan<char> formattedMessage)
    {
        var location = level switch
        {
            LogLevel.Info => Console.Out,
            _ => Console.Error
        };

        var levelString = level switch
        {
            LogLevel.Info => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Exception => "EXCEPTION",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };
        
        location.Write($"[{levelString}] [{category}] ");
        location.WriteLine(formattedMessage);
    }
}