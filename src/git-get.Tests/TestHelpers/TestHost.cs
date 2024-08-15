using GitGet.Services;

using InMemLogger;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitGet.Tests.TestHelpers;

public class TestHost : IDisposable
{
    private readonly ServiceProvider _svc;

    private readonly FakeConsole _console = new();

    public TestHost(Action<IServiceCollection>? cfg = null)
    {
        var cont = new ServiceCollection()
            .AddLogging(x => {
                x.AddInMemory()
                 .AddDebug()
                 .AddConsole();

                x.AddFakeLogging(fakeCfg => {
                    fakeCfg.OutputSink = l => LogEntries.Add(l);
                });

                x.AddFilter("", LogLevel.Trace);
            })
            .AddSingleton(x => x.GetRequiredService<ILoggerFactory>().CreateLogger(""));

        cont.AddSingleton<IConsole>(_console);

        cfg?.Invoke(cont);

        _svc = cont.BuildServiceProvider();
    }

    public T Get<T>(object? key = null) where T : class =>
        key is null ? _svc.GetRequiredService<T>() :
            _svc.GetRequiredKeyedService<T>(key);

    public readonly List<string> LogEntries = [];

    public string StdOut => _console.StdOut;

    public string StdError => _console.StdError;

    public void Dispose()
    {
        _svc.Dispose();

        GC.SuppressFinalize(this);
    }
}