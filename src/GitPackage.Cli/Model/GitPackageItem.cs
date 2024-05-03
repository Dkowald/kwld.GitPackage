namespace GitPackage.Cli.Model;

/// <summary>
/// 
/// </summary>
internal class GitPackageItem
{
    /// <summary>
    /// Url to the source repository: e.g.
    /// https://github.com/Dkowald/kwld.Xunit.Ordering.git
    /// Note the leading https:// can be removed if desired
    /// </summary>
    public string Include { get; set; }

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
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public string Path { get; set; }
}