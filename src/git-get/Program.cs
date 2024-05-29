using GitGet.Model;
using GitPackage.Cli.Model;
using GitPackage.Cli.Model.AppLogging;
using GitPackage.Cli.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitGet;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var logLevel = Args.ReadLogLevel(args);

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

        await using var svc = cont.BuildServiceProvider();

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

        var cache = new RepositoryCache(log, svc.GetRequiredService<IFileSystem>(), parsedArgs.Cache);

        if (parsedArgs.Action == Actions.Get)
        {
            var action = new Get(log, cache);

            return await action.Run(parsedArgs);
        }

        //todo: other actions.
        return -1;
    }
}