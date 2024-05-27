using GitPackage.Cli.Model;
using GitPackage.Cli.Model.AppLogging;
using GitPackage.Cli.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitPackage.Cli;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var argInfo = new Config().Read(args);

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

            log.AddFilter("", argInfo.LogLevel);
            //log.AddFilter<ConsoleLoggerProvider>("", argInfo.LogLevel);
        });

        await using var svc = cont.BuildServiceProvider();

        var appLog = svc.GetRequiredService<ILoggerFactory>().CreateLogger("");
        var files = svc.GetRequiredService<IFileSystem>();

        if (argInfo.Errors.Any())
        {
            foreach (var item in argInfo.Errors)
            {
                appLog.LogError(item);
            }

            return 1;
        }

        if (argInfo.ShowVersion) 
            return await new Help(appLog).Run();
        
        var config = new AppConfig(appLog, files, argInfo);

        var result = await new SyncFilesWithPackage(appLog)
            .Run(config);

        return result;
    }
}