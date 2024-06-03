using GitPackage.Cli.Model;

using Microsoft.Extensions.Logging;

namespace GitGet.Model;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// TODO: consider represent the status file as html,
/// so then I could just open it in browser to see info
/// </remarks>
internal record GitPackageStatusFile
{
    internal const string StatusFileName = ".gitget";

    public static GitPackageStatusFile? LoadIfFound(ILogger appLog, IDirectoryInfo dataFolder)
    {
        var file = dataFolder.GetFile(StatusFileName);
        if (!file.Exists) return null;
        return Load(appLog, file);
    }

    public static GitPackageStatusFile? Load(ILogger appLog, IFileInfo dataFile)
    {
        if (!dataFile.Exists)
        {
            appLog.LogError("Status file missing: {StatusFile}", dataFile.FullName);
            return null;
        }

        Uri? origin = null;
        GitRef? version = null;
        GetFilter? filter = null;
        string? commit = null;

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

            if (key.Same(nameof(Commit)))
            {
                commit = value.IsNullOrWhiteSpace() ? null : value;
                continue;
            }
        }

        if (origin is null || version is null || filter is null)
        {
            appLog.LogError($"{StatusFileName} invalid");
            return null;
        }

        return new(dataFile, origin, version, filter)
        {
            Commit = commit
        };
    }

    public GitPackageStatusFile(IDirectoryInfo targetFolder, Uri origin, GitRef version, GetFilter filter)
        : this(targetFolder.GetFile(StatusFileName), origin, version, filter) { }

    public GitPackageStatusFile(IFileInfo statusFile, Uri origin, GitRef version, GetFilter filter)
    {
        BackingFile = statusFile;
        Origin = origin;
        Version = version;
        Filter = filter;
    }

    public GitPackageStatusFile Write(ILogger log)
    {
        var content = new[]
        {
            $"{nameof(Origin)} = {Origin}",
            $"{nameof(Version)} = {Version.Version}",
            $"{nameof(Filter)} = {Filter}",
            $"{nameof(Commit)} = {Commit}",
        };

        log.LogDebug("Updating status file: {statusFile}", BackingFile.FullName);

        BackingFile
            .EnsureDirectory()
            .WriteAllLines(content);

        return this;
    }

    public IFileInfo BackingFile { get; }

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
    public string Path => BackingFile.DirectoryName!;

    /// <summary>
    /// The commit used for current files, if files have been collected
    /// </summary>
    public string? Commit { get; set; }
}