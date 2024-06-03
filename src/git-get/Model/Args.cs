using GitGet.Utility;

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

    public static readonly string DefaultCacheFolderName = ".gitpackages";

    public static LogLevel ReadLogLevel(string[] args)
    {
        var entry = args.LastOrDefault(x => x.StartsWith(LogLevelKey))?[LogLevelKey.Length..]
            ?? "w";

        if (entry.Same("c")) return LogLevel.Critical;
        if (entry.Same("e")) return LogLevel.Error;
        if (entry.Same("w")) return LogLevel.Warning;
        if (entry.Same("i")) return LogLevel.Information;
        if (entry.Same("d")) return LogLevel.Debug;
        if (entry.Same("t")) return LogLevel.Trace;

        throw new Exception($"Unknown LogLevel '{entry}'");
    }

    public static Args? Load(IFileSystem files, ILogger log, LogLevel logLevel, string[] inputArgs)
    {
        ActionOptions? action = null;
        IDirectoryInfo? targetPath = null;
        Uri? origin = null;
        GitRef? version = null;
        GetFilter? filter = null;
        IDirectoryInfo? cache = null;
        ForceOption? force = null;

        if (inputArgs.Length == 0)
        {
            log.LogTrace("No arguments; get using the current folder.");
            return new(logLevel, ActionOptions.Get, files.Current(), DefaultCache(files, log));
        }

        var idx = 1;

        var arg0 = inputArgs.First();

        if (arg0.Same("init"))
        { action = ActionOptions.Init; }
        else if (arg0.Same("info"))
        { action = ActionOptions.Info; }
        else if (arg0.Same("where"))
        { action = ActionOptions.Where; }
        else
        {
            action = ActionOptions.Get;
            if (arg0.StartsWith("--"))
            {
                idx = 0;
                targetPath = files.Current();
                log.LogInformation("No action provided, using get with current directory");
            }
            else
            {
                var isFile = files.Current().GetFile(arg0).Exists;
                if (isFile)
                {
                    log.LogError("Target path should be a directory, not file");
                    return null;
                }
                log.LogTrace("Target path set from action");
                targetPath = files.Current().GetFolder(arg0);
            }
        }

        if (action is null) throw new Exception("Failed resolve action");

        for (; idx < inputArgs.Length; idx++)
        {
            var next = inputArgs[idx];

            if (next.StartsWith(OrignKey))
            {
                if (origin is not null)
                {
                    log.LogError("Origin url already provided");
                    return null;
                }
                var value = next[OrignKey.Length..];
                if (!Uri.TryCreate(value, UriKind.Absolute, out origin))
                {
                    log.LogError("Invalid Origin '{origin}' must be a valid url to the repository", value);
                    return null;
                }
                continue;
            }

            if (next.StartsWith(VersionKey))
            {
                if (version is not null)
                {
                    log.LogError("Version already provided");
                    return null;
                }

                var value = next[VersionKey.Length..];
                (var error, version) = GitRef.TryRead(value);
                if (version is null)
                {
                    log.LogError("Invalid version {version}", value);
                    log.LogInformation("Version parse error: {error}", error);
                }
                continue;
            }

            if (next.StartsWith(FilterKey))
            {
                if (filter is not null)
                {
                    log.LogError("Filter already provided");
                    return null;
                }
                var value = next[FilterKey.Length..];
                try
                {
                    filter = new(value);
                }
                catch (Exception ex)
                {
                    log.LogError("Invalid filter : {filter}", value);
                    log.LogInformation(ex, $"Failed create {nameof(GetFilter)}");
                }
                continue;
            }

            if (next.StartsWith(CacheKey))
            {
                if (cache is not null)
                {
                    log.LogError("Cache already provided");
                    return null;
                }

                var value = next[ForceKey.Length..];
                if (files.Current().GetFile(value).Exists)
                {
                    log.LogError("Cache folder cannot be a file");
                    return null;
                }
                cache = files.Current().GetFolder(value);
                continue;
            }

            if (next.StartsWith(ForceKey))
            {
                var value = next[ForceKey.Length..];

                if (!Enum.TryParse<ForceOption>(value, true, out var forceValue))
                {
                    log.LogError("Invalid force option {force}", value);
                    return null;
                }

                force = forceValue;

                continue;
            }

            if (next.StartsWith(TargetPathKey))
            {
                if (targetPath is not null)
                {
                    log.LogError("TargetPath already provided");
                    return null;
                }
                var value = next[TargetPathKey.Length..];
                if (files.Current().GetFile(value).Exists)
                {
                    log.LogError("Target path cannot be a file");
                    return null;
                }
                targetPath = files.Current().GetFolder(value);
                continue;
            }

            if (next.StartsWith(LogLevelKey)) continue;

            log.LogError("Unknown option: {option}", next);
            return null;
        }

        if (targetPath is null)
        {
            log.LogInformation("Setting target path to current");
            targetPath = files.Current();
        }

        cache ??= DefaultCache(files, log);

        return new(logLevel, action.Value, targetPath, cache)
        {
            Origin = origin,
            Version = version,
            Filter = filter,
            Force = force
        };
    }

    public Args(LogLevel logLevel, ActionOptions action, IDirectoryInfo targetPath, IDirectoryInfo cache)
    {
        LogLevel = logLevel;
        Action = action;
        TargetPath = targetPath;
        Cache = cache;
    }

    public LogLevel LogLevel { get; init; }

    public ActionOptions Action { get; init; }

    public IDirectoryInfo TargetPath { get; init; }

    public Uri? Origin { get; init; }

    public GitRef? Version { get; init; }

    public GetFilter? Filter { get; init; }

    public IDirectoryInfo Cache { get; init; }

    public ForceOption? Force { get; init; }

    private static IDirectoryInfo DefaultCache(IFileSystem files, ILogger log)
    {
        var home = files.TryGetHome();

        if (home is null)
        {
            log.LogWarning("No home directory found!, using cwd");
            home = files.Current();
        }

        var cache = home.GetFolder(DefaultCacheFolderName);

        log.LogDebug("Cache path: '{cache}'", cache.FullName);

        return cache;
    }
}