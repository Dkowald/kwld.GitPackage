using GitPackage.Cli.Model;
using GitPackage.Cli.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitPackage.Cli;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var files = new FileSystem();

        var argInfo = new Config().Read(args);
        
        var logFactory = LoggerFactory.Create(cfg =>
        {
            cfg.AddConsole(x =>
            {
                x.LogToStandardErrorThreshold = LogLevel.Error;
            });

            //todo: custom formatter, or something to get rid of the eventid in console out
            cfg.AddSimpleConsole(x =>
                {
                    x.SingleLine = true;
                    x.TimestampFormat = "HH:mm:ss ";
                    x.IncludeScopes = false;
                });
                cfg.AddDebug();
                cfg.AddFilter<ConsoleLoggerProvider>("", argInfo.LogLevel);
        });
    
        var appLog = logFactory.CreateLogger("");

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