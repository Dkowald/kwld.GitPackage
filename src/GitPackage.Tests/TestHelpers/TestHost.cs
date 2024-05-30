using InMemLogger;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.TestHelpers;

public class TestHost : IDisposable
{
    private readonly ServiceProvider _svc;

    public TestHost()
    {
        var cont = new ServiceCollection()
            .AddLogging(cfg =>
            {
                cfg.AddInMemory()
                    .AddDebug()
                    .AddConsole();

                cfg.AddFilter("", LogLevel.Trace);
            })
            .AddSingleton(x => x.GetRequiredService<ILoggerFactory>().CreateLogger(""));

        _svc = cont.BuildServiceProvider();
    }

    public T Get<T>(object? key = null) where T:class => 
        key is null? _svc.GetRequiredService<T>() :
            _svc.GetRequiredKeyedService<T>(key);


    public void Dispose()
    {
        _svc.Dispose();
    }
}