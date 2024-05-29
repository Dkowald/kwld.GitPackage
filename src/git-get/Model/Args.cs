using GitPackage.Cli.Model;
using Microsoft.Extensions.Logging;

namespace GitGet.Model;

internal class Args
{
    private const string LogLevelKey = "--log-level:";
    private const string OrignKey = "--origin:";
    private const string VersionKey = "--version:";
    private const string FilterKey = "--filter:";
    private const string CacheKey = "--cache:";
    private const string ForceKey = "--force:";
    private const string TargetPathKey = "--target-path:";

    public static LogLevel ReadLogLevel(string[] args)
    {
        var entry = args.LastOrDefault(x => x.StartsWith(LogLevelKey))?[LogLevelKey.Length..]
            ??"w";

        if (entry.Same("c")) return LogLevel.Critical;
        if (entry.Same("e")) return LogLevel.Error;
        if (entry.Same("w")) return LogLevel.Warning;
        if (entry.Same("i")) return LogLevel.Information;
        if (entry.Same("d")) return LogLevel.Debug;
        if (entry.Same("t")) return LogLevel.Trace;

        throw new Exception($"Unknown LogLevel '{entry}'");
    }

    public static Args? Load(IFileSystem files, ILogger log, LogLevel logLevel, string[] args)
    {
        if (args.Length == 0)
        {
            log.LogTrace("No arguments; get using the current folder.");
            return new(logLevel, Actions.Get, files.Current());
        }

        var idx = 1;

        Actions? action;
        IDirectoryInfo? targetPath = null;

        var arg0 = args[0];
        if (arg0.Same("init"))
        { action = Actions.Init; }
        else if (arg0.Same("info"))
        { action = Actions.Info; }
        else if (arg0.Same("where"))
        { action = Actions.Where; }
        else{ action = Actions.Get;
          
            if (!arg0.StartsWith("--"))
            {
                var isFile = files.Current().GetFile(arg0).Exists;
                if (isFile)
                {
                    log.LogError("Target path should be a directory, not file");
                    return null;
                }
                log.LogDebug("Target path set from action");
                targetPath = files.Current().GetFolder(arg0);
            }
        }
        
        if (action is null) throw new Exception("Failed resolve action");

        if (targetPath is null)
        {
            log.LogDebug("TargetPath set to current directory");
            targetPath = files.Current();
        }
        
        var result = new Args(logLevel, action.Value, targetPath)
        {
            TargetPath = targetPath
        };

        for (; idx < args.Length; idx++)
        {
            var next = args[idx];

            if (next.StartsWith(OrignKey))
            {
                var value = next[OrignKey.Length..];
                if (!Uri.TryCreate(value, UriKind.Absolute, out var url))
                {
                    log.LogError("Invalid Origin '{origin}' must be a valid url to the repository", value);
                    return null;
                }
                result.Origin = url; continue;
            }

            if (next.StartsWith(VersionKey))
            {
                var value = next[VersionKey.Length..];
                (var error, result.Version) = GitRef.TryRead(value);
                if (result.Version is null)
                {
                    log.LogError("Invalid version {version}", value);
                    log.LogInformation("Version parse error: {error}", error);
                }
            }

            if (next.StartsWith(FilterKey))
            {
                var value = next[FilterKey.Length..];
                try
                {
                    result.Filter = new(value);
                }
                catch(Exception ex)
                {
                    log.LogError("Invalid filter : {filter}", value);
                    log.LogInformation(ex, $"Failed create {nameof(GetFilter)}");
                }
            }

            if (next.StartsWith(CacheKey))
            {
                var value = next[ForceKey.Length..];
                if (files.Current().GetFile(value).Exists)
                {
                    log.LogError("Cache folder cannot be a file");
                    return null;
                }

                result.Cache = files.Current().GetFolder(value); continue;
            }

            if (next.StartsWith(ForceKey))
            {
                var value = next[ForceKey.Length..];

                if (!Enum.TryParse(value, true, out ForceOption force))
                {
                    log.LogError("Invalid force option {force}", value);
                    return null;
                }

                result.Force = force;
                continue;
            }

            if (next.StartsWith(TargetPathKey))
            {
                if (result.Action == Actions.Get)
                {
                    log.LogWarning("Target path already provided, ignoring {option}", TargetPathKey);
                }
                else
                {
                    var value = next[TargetPathKey.Length..];
                    if (files.Current().GetFile(value).Exists)
                    {
                        log.LogError("Target path cannot be a file");
                        return null;
                    }

                    result.TargetPath = files.Current().GetFolder(value);
                    continue;
                }
            }

            //set as 
            if(next.StartsWith(LogLevelKey))continue;

            log.LogError("Unknown option: {option}", next);
            return null;
        }

        return result;
    }

    public Args(LogLevel logLevel, Actions action, IDirectoryInfo targetPath)
    { 
        LogLevel = logLevel;
        Action = action;
        TargetPath = targetPath;
    }

    public LogLevel LogLevel { get; private set; }

    public Actions Action { get; private set; }

    public IDirectoryInfo TargetPath { get; private set; }
    
    public Uri? Origin { get; private set; }

    public GitRef? Version { get; private set; }

    public GetFilter? Filter { get; private set; }

    public IDirectoryInfo? Cache { get; private set; }

    public ForceOption? Force { get; private set; }
}