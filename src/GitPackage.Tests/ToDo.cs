using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests;

public class ToDo
{
    [Fact(Skip = "Todo")]
    public void GetWithSubModule()
    {
        //Support using a git repo as a 
        // pinned package repository.
        
        //create sub-repo.
        //add sub repo to main, for specific sub-repo tag
        //gitget should include sub-repo files.
    }
    
    [Fact]
    public void CloneWithStructure()
    {
        var logFactory = LoggerFactory.Create(cfg => {
            cfg.AddConsole();
            cfg.AddDebug();

            cfg.AddFilter<ConsoleLoggerProvider>("GitGet", LogLevel.Information);
        });

        var appLog = logFactory.CreateLogger("GitGet");

        appLog.LogInformation("GitGet");

    }

    [Fact]
    public void RepoContainsVersionFile()
    {
        //if the repo include the
        //verison file used by this system.
        //then error.
    }
}