using GitPackage.Cli.Model;

using Microsoft.Extensions.Logging;

namespace GitGet.Model;

/// <summary>
/// Load / save config and status data for specified root folder.
/// </summary>
/// <remarks>
/// TODO: extract simpl .env file reader ability (can i use lib?)
/// </remarks>
internal record StatusFile
{
    internal const string StatusFolder = ".gitget";
    internal const string ConfigFile = "config";
    internal const string CommitFile = "commit";

    public static StatusFile? LoadIfFound(ILogger appLog, IDirectoryInfo dataFolder)
    {
        if (!dataFolder.Exists()) return null;

        return Load(appLog, dataFolder);
    }

    private static StatusFile? Load(ILogger appLog, IDirectoryInfo dataFolder)
    {
        if (!dataFolder.Exists)
        {
            appLog.LogError("Status folder not found: {StatusFolder}", dataFolder.FullName);
            return null;
        }

        Uri? origin = null;
        GitRef? version = null;
        GetFilter? filter = null;
        
        var dataFile = dataFolder.GetFile(StatusFolder, ConfigFile);
        if (!dataFile.Exists)
        {
            appLog.LogTrace($"{StatusFolder}/{ConfigFile} not found");
            return null;
        }

        foreach (var line in dataFile.ReadAllLines())
        {
            if (line.IsNullOrWhiteSpace()) continue;

            var idx = line.IndexOf('=');
            if (idx < 0 || idx == line.Length - 1)
            {
                appLog.LogError("Status File corrupt: {StatusFile}", dataFile.FullName);
                appLog.LogInformation("Deleting corrupted status file");
                dataFile.Delete();
            }

            var key = line[..idx++].Trim();
            var value = line[idx..].Trim();

            if (key.Same(nameof(Origin)))
            {
                if (!Uri.TryCreate(value, UriKind.Absolute, out origin))
                {
                    appLog.LogError("corrupt file: origin invalid");
                }
                continue;
            }

            if (key.Same(nameof(Version)))
            {
                (var error, version) = GitRef.TryRead(value);
                if (version is null)
                {
                    appLog.LogError("corrupt file: invalid version: {error}", error);
                }
                continue;
            }

            if (key.Same(nameof(Filter)))
            {
                filter = new(value);
                continue;
            }
        }

        if (origin is null || version is null || filter is null)
        {
            appLog.LogError($"{StatusFolder}/{ConfigFile} invalid");
            return null;
        }

        var statusFile = dataFolder.GetFile(StatusFolder, CommitFile);
        string? commit = null;
        if(statusFile.Exists)
            foreach(var line in statusFile.ReadAllLines())
            {
                if (line.IsNullOrWhiteSpace()) continue;
                var idx = line.IndexOf('=');
                if (idx < 0 || idx == line.Length - 1)
                {
                    appLog.LogError("Status File corrupt: {StatusFile}", dataFile.FullName);
                    appLog.LogInformation("Deleting corrupted status file");
                    dataFile.Delete();
                }

                var key = line[..idx++].Trim();
                var value = line[idx..].Trim();

                if (key.Same(nameof(Commit)))
                {
                    commit = value.IsNullOrWhiteSpace() ? null : value;
                    continue;
                }
            }

        return new(dataFolder, origin, version, filter)
        {
            Commit = commit
        };
    }

    public StatusFile(IDirectoryInfo targetFolder, Uri origin, GitRef version, GetFilter filter)
    {
        TargetPath = targetFolder;

        Origin = origin;
        Version = version;
        Filter = filter;
    }

    /// <summary>
    /// Persist to backing data file(s)
    /// </summary>
    public StatusFile Write(ILogger log)
    {
        var content = new[]
        {
            $"{nameof(Origin)} = {Origin}",
            $"{nameof(Version)} = {Version.Version}",
            $"{nameof(Filter)} = {Filter}",
        };

        var configFile = TargetPath.GetFile(StatusFolder, ConfigFile).EnsureDirectory();
        log.LogDebug("Updating config file: {configFile}", configFile.FullName);
        configFile.WriteAllLines(content);

        var statusFile = TargetPath.GetFile(StatusFolder, CommitFile).EnsureDirectory();
        log.LogDebug("Updating status file: {statusFile}", statusFile.FullName);

        if (Commit.IsNullOrWhiteSpace())
            statusFile.EnsureDelete();
        else
            statusFile.WriteAllLines([$"{nameof(Commit)} = {Commit}"]);

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
    public GetFilter Filter { get; set; } = new();

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public IDirectoryInfo TargetPath { get; init;}

    /// <summary>
    /// The commit used for current files, if files have been collected
    /// </summary>
    public string? Commit { get; set; }
}