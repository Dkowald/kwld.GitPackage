using GitGet.Actions;
using GitGet.Utility;

using Microsoft.Extensions.Logging;

namespace GitGet.Model;

internal record Args
{
    private const string LogLevelKey = "--log-level:";
    private const string OriginKey = "--origin:";
    private const string VersionKey = "--version:";
    private const string FilterKey = "--filter:";
    private const string IgnoreKey = "--ignore:";
    private const string GetRootKey = "--get-root:";
    private const string CacheKey = "--cache:";
    private const string ForceKey = "--force:";
    private const string TargetPathKey = "--target-path:";
    private const string UsernameKey = "--user:";
    private const string PasswordKey = "--password:";

    public static readonly string DefaultCacheFolderName = ".gitpackages";
    public static readonly LogLevel DefaultLogLevel = LogLevel.Information;

    public static LogLevel ReadLogLevel(string[] args)
    {
        var entry = args.LastOrDefault(x => x.StartsWith(LogLevelKey))?[LogLevelKey.Length..];

        if(entry is null) return DefaultLogLevel;
        if(entry.Same("c")) return LogLevel.Critical;
        if(entry.Same("e")) return LogLevel.Error;
        if(entry.Same("w")) return LogLevel.Warning;
        if(entry.Same("i")) return LogLevel.Information;
        if(entry.Same("d")) return LogLevel.Debug;
        if(entry.Same("t")) return LogLevel.Trace;

        throw new Exception($"Unknown LogLevel '{entry}'");
    }

    public static Args? Load(IFileSystem files, ILogger log, LogLevel logLevel, params string[] inputArgs)
    {
        ActionOptions? action = null;
        IDirectoryInfo? targetPath = null;
        Uri? origin = null;
        GitRef? version = null;
        GlobFilter? filter = null;
        GlobFilter? ignore = null;
        RootPath? root = null;
        IDirectoryInfo? cache = null;
        ForceOption? force = null;
        string? user = null;
        string? password = null;

        if(inputArgs.Length == 0) {
            log.LogTrace("No arguments; get using the current folder.");
            return new(logLevel, ActionOptions.Get, files.Current(), DefaultCache(files, log));
        }

        var idx = 1;

        var arg0 = inputArgs.First();
        
        if(arg0.Same(nameof(Init))) { action = ActionOptions.Init; } 
        else if(arg0.Same(nameof(Info))) { action = ActionOptions.Info; } 
        else if(arg0.Same(nameof(Where))) { action = ActionOptions.Where; } 
        else if(arg0.Same(nameof(About))) {action = ActionOptions.About;} 
        else if(arg0.StartsWith("--")) {idx = 0;} 
        else {
            var isFile = files.Current().GetFile(arg0);

            targetPath = isFile.Exists ? isFile.Directory! : files.Current().GetFolder(arg0);

            if(isFile.Exists) {
                log.LogWarning("Target path is a file, using its containing directory");
            } else {
                log.LogTrace("Target path set from action");
            }
        }

        action ??= ActionOptions.Get;

        for(; idx < inputArgs.Length; idx++) {
            var next = inputArgs[idx];

            if(next.StartsWith(OriginKey)) {
                if(origin is not null) {
                    log.LogError("argument already provided - Origin url");
                    return null;
                }
                var value = next[OriginKey.Length..];
                if(!Uri.TryCreate(value, UriKind.Absolute, out origin)) {
                    log.LogError("Invalid Origin '{origin}' must be a valid url to the repository", value);
                    return null;
                }
                continue;
            }

            if(next.StartsWith(VersionKey)) {
                if(version is not null) {
                    log.LogError("argument already provided - Version");
                    return null;
                }

                var value = next[VersionKey.Length..];
                (var error, version) = GitRef.TryRead(value);
                if(version is null) {
                    log.LogError("Invalid version {version}", value);
                    log.LogError("Version parse error: {error}", error);
                    return null;
                }
                continue;
            }

            if(next.StartsWith(FilterKey)) {
                if(filter is not null) {
                    log.LogError("argument already provided - Filter");
                    return null;
                }
                var value = next[FilterKey.Length..];
                try {
                    filter = new(value);
                } catch(Exception ex) {
                    log.LogError("Invalid filter : {filter}", value);
                    log.LogError(ex, $"Failed create {nameof(GlobFilter)}");
                    return null;
                }
                continue;
            }

            if(next.StartsWith(IgnoreKey)) {
                if(ignore is not null) {
                    log.LogError("argument already provided - Ignore");
                    return null;
                }
                var value = next[IgnoreKey.Length..];
                try {
                    ignore = new(value);
                } catch(Exception ex) {
                    log.LogError("Invalid ignore: {ignore}", value);
                    log.LogError(ex, $"Failed create {nameof(Ignore)}");
                    return null;
                }
                continue;
            }

            if(next.StartsWith(GetRootKey)) {
                if(root is not null) {
                    log.LogError("argument already provided - GetRoot");
                    return null;
                }
                var value = next[GetRootKey.Length..];
                try {
                    root = new(value);
                } catch(Exception ex) {
                    log.LogError("Invalid get-root: {get-root}", value);
                    log.LogError(ex, $"Failed create {nameof(GetRoot)}");
                    return null;
                }
                continue;
            }

            if(next.StartsWith(CacheKey)) {
                if(cache is not null) {
                    log.LogError("argument already provided - Cache");
                    return null;
                }

                var value = next[ForceKey.Length..];
                if(files.Current().GetFile(value).Exists) {
                    log.LogError("Cache folder cannot be a file");
                    return null;
                }
                cache = files.Current().GetFolder(value);
                continue;
            }

            if(next.StartsWith(ForceKey)) {
                var value = next[ForceKey.Length..];

                if(!Enum.TryParse<ForceOption>(value, true, out var forceValue)) {
                    log.LogError("Invalid force option {force}", value);
                    return null;
                }

                force = forceValue;

                continue;
            }

            if(next.StartsWith(TargetPathKey)) {
                if(targetPath is not null) {
                    log.LogError("argument already provided - TargetPath");
                    return null;
                }
                var value = next[TargetPathKey.Length..];
                if(files.Current().GetFile(value).Exists) {
                    log.LogError("Target path cannot be a file");
                    return null;
                }
                targetPath = files.Current().GetFolder(value);
                continue;
            }

            if(next.StartsWith(LogLevelKey)) continue;

            if(next.StartsWith(PasswordKey)) {
                var value = next[PasswordKey.Length..];
                password = value;
                continue;
            }

            if(next.StartsWith(UsernameKey)) {
                var value = next[UsernameKey.Length..];
                user = value;
                continue;
            }

            log.LogError("Unknown option: {option}", next);
            return null;
        }

        targetPath ??= files.Current();

        cache ??= DefaultCache(files, log);

        return new(logLevel, action.Value, targetPath, cache) {
            _password = password,
            Origin = origin,
            Version = version,
            Filter = filter,
            Ignore = ignore,
            GetRoot = root,
            Force = force,
            User = user
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

    public IDirectoryInfo Cache { get; init; }

    public Uri? Origin { get; init; }

    public GitRef? Version { get; init; }

    public GlobFilter? Filter { get; init; }

    public GlobFilter? Ignore { get; init; }

    public RootPath? GetRoot { get; init; }

    public ForceOption? Force { get; init; }

    public string? User { get; init; }

    //so its more difficult to accidentally use. 
    private string? _password;
    public bool HasPassword => _password != null;
    internal string? UsePassword() => _password;

    private static IDirectoryInfo DefaultCache(IFileSystem files, ILogger log)
    {
        var home = files.TryGetHome();

        if(home is null) {
            log.LogWarning("No home directory found!, using cwd");
            home = files.Current();
        }

        var cache = home.GetFolder(DefaultCacheFolderName);

        return cache;
    }
}