using Microsoft.Build.Framework;

namespace GitPackage.Model;

/// <summary>
/// A git package build item.
/// </summary>
public class GitPackageItem
{
    public GitPackageItem(){}

    /// <summary>
    /// Url to the source repository: e.g.
    /// https://github.com/Dkowald/kwld.Xunit.Ordering.git
    /// Note the leading https:// can be removed if desired
    /// </summary>
    public string Include { get; set; }

    /// <summary>
    /// The Version; corresponds to a git commit.
    /// Can be either a branch or a tag:
    /// tag/v1.0.0-preview1
    /// branch/dev
    /// If not prefixed with branch or tag; then it is assumed to be tag.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Optional glob filter on source files.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public string Path { get; set; }

    //Attached
    
    public string RepositoryRoot { get; set; }

    /// <summary>
    /// True when the target already exists, and has the expected file(s)
    /// </summary>
    public bool TargetExists { get; set; }
}