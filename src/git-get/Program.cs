using GitGet.Actions;
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

        if(parsedArgs is null) {
            log.LogCritical("Command line parse failed; aborting");
            return 1;
        }

        IAction action = parsedArgs.Action switch {
            ActionOptions.Get =>
                svc.GetRequiredService<GetAction>(),
            ActionOptions.Init =>
                svc.GetRequiredService<Init>(),
            ActionOptions.About =>
                svc.GetRequiredService<About>(),
            ActionOptions.Info =>
                svc.GetRequiredService<Info>(),
            ActionOptions.Where =>
                svc.GetRequiredService<Where>(),

            _ => throw new Exception($"Unknown action: {parsedArgs.Action}")
        };

        if(action is not Where)
            log.LogTrace("Running action: {action}", parsedArgs.Action);

        var exitCode = await action.Run(parsedArgs);

        return exitCode;
    }

    private static ServiceCollection Container(LogLevel logLevel)
    {
        var cont = new ServiceCollection();

        cont.AddSingleton(TimeProvider.System)
            .AddSingleton<IFileSystem, FileSystem>();

        cont.AddLogging(log => {
            log.AddDebug()
               .AddConsole(x => {
                   x.LogToStandardErrorThreshold = LogLevel.Error;
                   x.FormatterName = nameof(AppLoggerFormatter);
               });

            log.AddConsoleFormatter<AppLoggerFormatter, SimpleConsoleFormatterOptions>();

            log.AddFilter("", logLevel);
        });

        //register default (app)logger
        cont.AddSingleton(ctx => ctx.GetRequiredService<ILoggerFactory>().CreateLogger(""));

        cont.AddSingleton<IConsole, ConsoleService>();

        cont.AddTransient<GetAction>();
        cont.AddTransient<Where>();
        cont.AddTransient<Info>();
        cont.AddTransient<Init>();
        cont.AddTransient<About>();

        return cont;
    }
}