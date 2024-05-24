using System.Reflection;
using GitPackage.Cli.Model;
using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Tasks;

internal class Help
{
    private readonly ILogger _appLog;
    public Help(ILogger appLog)
    {
        _appLog = appLog;
    }
    public Task<int> Run()
    {
        var ver = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion!;

        var txt = new string[]
        {"Git Get"
         ,"-------"
         , Config.HomeRepository
         ,"-------"
         ,$"Version: {ver}"
         ,"-------"
         ," ARGS  "
         ,"-v Show version and usage"
         ,"-l [LoggingLevel]: [t]race, [d]ebug, [i]nfo, [w]arn, [e]rror"
         ,"-c [Cache] : (optional) local cache for cloned repositories"
         ,"-f [StatusFile] : gitget status file used for details"
         ,"---------------------------------------------------------"
         ," Status File"
         ,"Simple text file with key=value lines"
         ,"Include=[repository-url]"
         ,"Version=[branch or tag to get]"
         ,"Filter=[glob filter(s), separated by ';']"
         ,"---------------------------------------------------------"
        };

        foreach (var line in txt)
        {
            Console.WriteLine(line);
        }
        
        return Task.FromResult(0);
    }
}