using Microsoft.Extensions.Logging;

namespace GitGet.Model;

/// <summary>
/// Load / save config and status data for specified root folder.
/// </summary>
internal class StatusFile
{
    internal const string FileName = ".gitget";
    
    public static async Task<(StatusFile? File, bool Changed)> TryLoadWithOverrides(ILogger log, Args args)
    {
        var stored = await TryLoad(log, args.TargetPath);
        
        Uri? origin;
        GitRef? version;
        GlobFilter? filter;
        GlobFilter? ignore;
        RootPath? getRoot;
        string? commit = stored?.Commit;

        bool changed = false;
        
        origin = args.Origin ?? stored?.Origin;
        changed |= origin != stored?.Origin;
        if (origin is null)
        {
            log.LogError("Cannot resolve Origin from status file or arguments");
            return (null, false);
        }

        version = args.Version ?? stored?.Version;
        changed |= version != stored?.Version;
        if (version is null)
        {
            log.LogError("Cannot resolve Version from status file or arguments");
            return (null, false);
        }

        filter = args.Filter ?? stored?.Filter;
        changed |= filter != stored?.Filter;
        if (filter is null)
        {
            log.LogWarning("No filter found from status file or arguments, using default");
            filter = GlobFilter.MatchAll;
            changed = true;
        }

        ignore = args.Ignore ?? stored?.Ignore;
        changed |= ignore != stored?.Ignore;

        getRoot = args.GetRoot;
        changed |= getRoot != stored?.GetRoot;
        
        if (changed) commit = null;
        
        var data = new StatusFile(args.TargetPath, origin, version, filter)
        { Commit = commit, GetRoot = getRoot, Ignore = ignore};

        return (data, changed);
    }

    public static async Task<StatusFile?> TryLoad(ILogger appLog, IDirectoryInfo dataFolder)
    {
        var statusFile = dataFolder.GetFile(FileName);

        if (!statusFile.Exists()) return null;

        Uri? origin = null;
        GitRef? version = null;
        GlobFilter? filter = null;
        GlobFilter? ignore = null;
        string? getRoot = null;
        string? commit = null;

        var lineNumber = 0;
        foreach (var line in await statusFile.ReadAllLinesAsync())
        {
            lineNumber++;
            if (line.IsNullOrWhiteSpace()) continue;

            var idx = line.IndexOf('=');
            if (idx < 0 || idx == line.Length - 1)
            {
                appLog.LogError("Status File corrupt: {StatusFile} #{LineNumber}",
                    statusFile.FullName, lineNumber);
            }

            var key = line[..idx++].Trim();
            var value = line[idx..].Trim();

            if (key.Same(nameof(Origin)))
            {
                if (!Uri.TryCreate(value, UriKind.Absolute, out origin))
                {
                    appLog.LogError("corrupt file: origin '{Origin}' invalid #{LineNumber}",
                        value, lineNumber);
                }
                continue;
            }

            if (key.Same(nameof(Version)))
            {
                (var error, version) = GitRef.TryRead(value);
                if (version is null)
                {
                    appLog.LogError("corrupt file: version: {error} #{LineNumber}", error, lineNumber);
                }
                continue;
            }

            if (key.Same(nameof(Filter)))
            {
                filter = new(value);
                continue;
            }

            if (key.Same(nameof(Ignore)))
            {
                ignore = value.IsNullOrEmpty()? null : new(value);
                continue;
            }

            if (key.Same(nameof(GetRoot)))
            {
                getRoot = value.IsNullOrWhiteSpace() ? null : value;
                continue;
            }

            if (key.Same(nameof(Commit)))
            {
                commit = value.IsNullOrWhiteSpace() ? null : value;
            }
        }

        if (origin is null || version is null || filter is null)
        {
            appLog.LogError("{StatusFile} invalid, missing required details", statusFile.FullName);
            return null;
        }

        return new StatusFile(dataFolder, origin, version, filter)
        {
            Commit = commit,
            Ignore = ignore,
            GetRoot = RootPath.TryParse(getRoot) ?? RootPath.Default
        };
    }

    public StatusFile(IDirectoryInfo targetFolder, Uri origin, GitRef version, GlobFilter filter)
    {
        TargetPath = targetFolder;

        Origin = origin;
        Version = version;
        Filter = filter;
    }

    /// <summary>
    /// Persist to backing data file(s)
    /// </summary>
    public async Task<StatusFile> Write(ILogger log)
    {
        var content = new List<string>
        {
            $"{nameof(Origin)} = {Origin}",
            $"{nameof(Version)} = {Version.Version}",
            $"{nameof(Filter)} = {Filter}",
            $"{nameof(Ignore)} = {Ignore}",
            $"{nameof(GetRoot)} = {GetRoot}"
        };

        var configFile = TargetPath.GetFile(FileName).EnsureDirectory();
        log.LogDebug("Updating config file: {configFile}", configFile.FullName);
        await configFile.WriteAllLinesAsync(content);

        if (!Commit.IsNullOrWhiteSpace())
            await configFile.AppendAllLinesAsync([$"{nameof(Commit)} = {Commit}"]);

        return this;
    }

    /// <summary>
    /// Url to the source repository: e.g.
    /// https://github.com/Dkowald/kwld.Xunit.Ordering.git
    /// Note the leading https:// can be removed if desired
    /// </summary>
    public Uri Origin { get; set; }

    /// <summary>
    /// The Version; corresponds to a git commit.
    /// Can be either a branch or a tag ref:
    /// tags/v1.0.0-preview1
    /// heads/dev
    /// If not prefixed with tags or heads; then it is assumed to be tags.
    /// </summary>
    public GitRef Version { get; set; }
    
    /// <summary>
    /// Optional glob filter on source files.
    /// Defaults to 
    /// </summary>
    public GlobFilter Filter { get; set; }

    /// <summary>
    /// Optional ignore filter
    /// </summary>
    public GlobFilter? Ignore { get; set; }

    /// <summary>
    /// Sub path in repository tree to use as root.
    /// </summary>
    public RootPath GetRoot { get; set; } = RootPath.Default;

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public IDirectoryInfo TargetPath { get; init;}

    /// <summary>
    /// The commit used for current files, if files have been collected
    /// </summary>
    public string? Commit { get; set; }

    public GetFilter GetFilter => new(Filter, Ignore);

    public bool Same(StatusFile? rhs)
    {
        if (rhs is null) return false;

        return
            Origin == rhs.Origin &&
            Version == rhs.Version &&
            Filter == rhs.Filter &&
            TargetPath.FullName == rhs.TargetPath.FullName &&
            GetRoot == rhs.GetRoot &&
            Commit == rhs.Commit;
    }
}