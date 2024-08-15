using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace GitGet.Model.AppLogging;

/// <summary>
/// Simplified console formatter,
/// to provide cleaner data for 
/// </summary>
internal class AppLoggerFormatter : ConsoleFormatter
{
    private readonly TimeProvider _clock;

    public AppLoggerFormatter(TimeProvider clock) : base(nameof(AppLoggerFormatter))
    {
        _clock = clock;
    }

    public override void Write<TState>(in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if(message.IsNullOrEmpty()) return;

        var level = logEntry.LogLevel.ToString().ToUpper();

        var when = _clock.GetUtcNow().ToString("u");

        var line = $"[{when}]:{level}:{message}";

        textWriter.WriteLine(line);
    }
}