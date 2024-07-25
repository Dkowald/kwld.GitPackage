using InMemLogger;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.TestHelpers;

public class TestHost : IDisposable
{
    private readonly ServiceProvider _svc;

    public TestHost(Action<IServiceCollection>? cfg = null)
    {
        var cont = new ServiceCollection()
            .AddLogging(cfg =>
            {
                cfg.AddInMemory()
                    .AddDebug()
                    .AddConsole();

                cfg.AddFakeLogging(x => { 
                    x.OutputSink = l => LogEntries.Add(l);
                });

                cfg.AddFilter("", LogLevel.Trace);
            })
            .AddSingleton(x => x.GetRequiredService<ILoggerFactory>().CreateLogger(""));

        cfg?.Invoke(cont);

        _svc = cont.BuildServiceProvider();
    }

    public T Get<T>(object? key = null) where T:class => 
        key is null? _svc.GetRequiredService<T>() :
            _svc.GetRequiredKeyedService<T>(key);

    public readonly List<string> LogEntries = [];

    public void Dispose()
    {
        _svc.Dispose();

        GC.SuppressFinalize(this);
    }
}