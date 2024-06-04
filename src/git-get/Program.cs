using GitGet.Model;
using GitGet.Model.AppLogging;
using GitGet.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitGet;

internal static class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var logLevel = Args.ReadLogLevel(args);

        await using var svc = Container(logLevel).BuildServiceProvider();

        var log = svc.GetRequiredService<ILogger>();

        var parsedArgs = Args.Load(
            svc.GetRequiredService<IFileSystem>(),
            svc.GetRequiredService<ILogger>(),
            logLevel, args);

        if (parsedArgs is null)
        {
            log.LogCritical("Command line parse failed; aborting");
            return 1;
        }

        var cache = new RepositoryCache(log, parsedArgs.Cache);

        if (parsedArgs.Action == ActionOptions.Get)
        {
            var action = new Actions.Get(log, cache);

            return await action.Run(parsedArgs);
        }

        if (parsedArgs.Action == ActionOptions.Where)
        {
            var action = new Actions.Where(log, svc.GetRequiredService<IConsole>());

            return await action.Run(parsedArgs);
        }

        if (parsedArgs.Action == ActionOptions.Info)
        {
            var action = new Actions.Info(log, svc.GetRequiredService<IConsole>());

            return await action.Run(parsedArgs);
        }

        //todo: other actions.
        return -1;
    }

    private static ServiceCollection Container(LogLevel logLevel)
    {
        var cont = new ServiceCollection();

        cont.AddSingleton(TimeProvider.System)
            .AddSingleton<IFileSystem, FileSystem>();

        cont.AddLogging(log =>
        {
            log.AddDebug()
                .AddConsole(x =>
                {
                    x.LogToStandardErrorThreshold = LogLevel.Error;
                    x.FormatterName = nameof(AppLoggerFormatter);
                });

            log.AddConsoleFormatter<AppLoggerFormatter, SimpleConsoleFormatterOptions>();

            log.AddFilter("", logLevel);
        });

        //register default (app)logger
        cont.AddSingleton(ctx => ctx.GetRequiredService<ILoggerFactory>().CreateLogger(""));

        cont.AddSingleton<IConsole, ConsoleService>();

        cont.AddTransient<Actions.Get>();
        cont.AddTransient<Actions.Where>();

        return cont;
    }
}