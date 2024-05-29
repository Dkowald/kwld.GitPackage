using GitGet.Model.AppLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Time.Testing;

//using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitPackage.Tests.Model.AppLogging;

public class AppLoggerFormatterTests
{
    public class CaptureIO : IDisposable
    {
        private readonly StringWriter _stdOut = new();
        private readonly StringWriter _stdError = new();

        public CaptureIO()
        {
            Console.Out.Flush();
            Console.SetOut(_stdOut);
            Console.SetError(_stdError);
            IsRedirecting = true;
        }

        public bool IsRedirecting { get; private set; }

        public string? StdOut { get; private set; }
        public string? StdError { get; private set; }

        public void Dispose()
        {
            _stdOut.Flush();
            StdOut = _stdOut.GetStringBuilder().ToString();
            _stdOut.Dispose();

            _stdError.Flush();
            StdError = _stdError.GetStringBuilder().ToString();
            _stdError.Dispose();

            Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
                { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError())
                { AutoFlush = true });

            IsRedirecting = false;
        }
    }

    [Fact]
    public async Task Log_()
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

    [Fact(Skip = "todo: fix so it capture console properly??")]
    public async Task Log_Console()
    {
        var cont = new ServiceCollection()
            .AddLogging(log =>
            {
                log.AddConsoleFormatter<AppLoggerFormatter, SimpleConsoleFormatterOptions>();
                log.AddConsole(x =>
                {
                    x.LogToStandardErrorThreshold = LogLevel.Error;
                    x.FormatterName = nameof(AppLoggerFormatter);
                });
                log.AddFilter("", LogLevel.Debug);
            })
            .AddSingleton<TimeProvider>(new FakeTimeProvider());

        var con = new CaptureIO();
        await using (var svc = cont.BuildServiceProvider())
        {
            var logger = svc.GetRequiredService<ILoggerFactory>().CreateLogger("");
            using (con)
            {
                logger.LogError("Test error");
                logger.LogInformation("Test info");
                logger.LogDebug("Test debug");
                logger.LogTrace("Test trace");
            }
        }

        Assert.Contains("Test info", con.StdOut);
        Assert.Contains("Test debug", con.StdOut);
        Assert.DoesNotContain("Test trace", con.StdOut);
        Assert.DoesNotContain("Test error", con.StdOut);

        Assert.Contains("Test error", con.StdError);
    }
}