using GitGet.Model.AppLogging;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace GitGet.Tests.Model.AppLogging;

public class AppLoggerFormatterTests
{
    [Fact]
    public async Task Write_()
    {
        var target = new AppLoggerFormatter(new FakeTimeProvider());

        await using var wr = new StringWriter();

        var logError = new LogEntry<string>(
            LogLevel.Error, "", 0, "Test error out", null,
            (s, _) => s);
        target.Write(logError, null, wr);

        var logInfo = new LogEntry<string>(
            LogLevel.Information, "", 0, "Test info out", null,
            (s, _) => s);
        target.Write(logInfo, null, wr);

        var logDebug = new LogEntry<string>(
            LogLevel.Debug, "", 0, "Test log debug", null,
            (s, _) => s);
        target.Write(logDebug, null, wr);

        await wr.FlushAsync();
        var txt = wr.GetStringBuilder().ToString();
        await Verify(txt);
    }
}