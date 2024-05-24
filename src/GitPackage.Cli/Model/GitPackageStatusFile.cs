using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Model;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// TODO: consider represent the status file as html,
/// so then I could just open it in browser to see info
/// </remarks>
internal record GitPackageStatusFile
{
    private readonly ILogger _appLog;
    internal const string StatusFileName = ".gitpackage";

    public static GitPackageStatusFile? Load(ILogger appLog, IDirectoryInfo dataFolder)
        => Load(appLog, dataFolder.GetFile(StatusFileName));

    public static GitPackageStatusFile? Load(ILogger appLog, IFileInfo dataFile)
    {
        var result = new GitPackageStatusFile(appLog, dataFile);

        if (!dataFile.Exists)
        {
            appLog.LogError("Status file missing: {StatusFile}", dataFile.FullName);
            return null;
        }

        foreach (var line in dataFile.ReadAllLines())
        {
            var idx = line.IndexOf('=');
            if (idx < 0 || idx == line.Length - 1)
            {
                appLog.LogError("Status File corrupt: {StatusFile}", dataFile.FullName);
            }

            var key = line[..idx++].Trim();
            var value = line[idx..].Trim();

            if (key.Same(nameof(Include)))
                result.Include = value;

            if (key.Same(nameof(Version)))
                result.Version = new(value);

            if (key.Same(nameof(Filter)))
                result.Filter = new(value);

            if (key.Same(nameof(Commit)))
                result.Commit = new(value);
        }

        return result;
    }

    public GitPackageStatusFile(ILogger appLog, IFileInfo statusFile)
    {
        _appLog = appLog;
        BackingFile = statusFile;
    }

    public GitPackageStatusFile(ILogger appLog, IDirectoryInfo folder) 
        : this(appLog, folder.GetFile(StatusFileName)){}

    public GitPackageStatusFile Write()
    {
        var content = new[]
        {
            $"{nameof(Include)} = {Include}",
            $"{nameof(Version)} = {Version}",
            $"{nameof(Filter)} = {Filter}",
            $"{nameof(Commit)} = {Commit}",
        };

        _appLog.LogDebug("Updating status file: {statusFile}", BackingFile.FullName);

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
    public string? Include { get; set; }

    /// <summary>
    /// The Version; corresponds to a git commit.
    /// Can be either a branch or a tag ref:
    /// tags/v1.0.0-preview1
    /// heads/dev
    /// If not prefixed with tags or heads; then it is assumed to be tags.
    /// </summary>
    public GitRef? Version { get; set; }

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