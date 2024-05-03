using Microsoft.Build.Framework;

namespace GitPackage.Tasks;

/// <summary>
/// A git package build item.
/// </summary>
public class GitPackageItem
{
    public readonly ITaskItem TaskItem;

    public GitPackageItem(ITaskItem item)
    { TaskItem = item; }

    /// <summary>
    /// Url to the source repository: e.g.
    /// https://github.com/Dkowald/kwld.Xunit.Ordering.git
    /// Note the leading https:// can be removed if desired
    /// </summary>
    public string Include 
    {
        get => TaskItem.GetMetadata(nameof(Include));
        set => TaskItem.SetMetadata(nameof(Include), value);
    }

    /// <summary>
    /// The Version; corresponds to a git commit.
    /// Can be either a branch or a tag:
    /// tag/v1.0.0-preview1
    /// branch/dev
    /// If not prefixed with branch or tag; then it is assumed to be tag.
    /// </summary>
    public string Version
    {
        get => TaskItem.GetMetadata(nameof(Version));
        set => TaskItem.SetMetadata(nameof(Version), value);
    }

    /// <summary>
    /// Optional glob filter on source files.
    /// </summary>
    public string Filter
    {
        get => TaskItem.GetMetadata(nameof(Filter));
        set => TaskItem.SetMetadata(nameof(Filter), value);
    }

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public string Path {
        get => TaskItem.GetMetadata(nameof(Path));
        set => TaskItem.SetMetadata(nameof(Path), value);
    }

    /// <summary>
    /// The root local repository cache folder.
    /// </summary>
    public string RepositoryRoot
    {
        get => TaskItem.GetMetadata(nameof(RepositoryRoot));
        set => TaskItem.SetMetadata(nameof(RepositoryRoot), value);
    }

    /// <summary>
    /// true when the target already exists, and has the expected file(s)
    /// </summary>
    public string TargetExists 
    {
        get => TaskItem.GetMetadata(nameof(TargetExists));
        set => TaskItem.SetMetadata(nameof(TargetExists), value);
    }
}