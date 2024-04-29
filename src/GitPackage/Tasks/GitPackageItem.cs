using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace GitPackage.Tasks;

/// <summary>
/// A git package build item.
/// </summary>
public class GitPackageItem
{
    private readonly ITaskItem _taskItem;

    public GitPackageItem(ITaskItem item)
    { _taskItem = item; }

    /// <summary>
    /// Url to the source repository: e.g.
    /// https://github.com/Dkowald/kwld.Xunit.Ordering.git
    /// Note the leading https:// can be removed if desired
    /// </summary>
    public string Include 
    {
        get => _taskItem.GetMetadata(nameof(Include));
        set => _taskItem.SetMetadata(nameof(Include), value);
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
        get => _taskItem.GetMetadata(nameof(Version));
        set => _taskItem.SetMetadata(nameof(Version), value);
    }

    /// <summary>
    /// Optional glob filter on source files.
    /// </summary>
    public string Filter
    {
        get => _taskItem.GetMetadata(nameof(Filter));
        set => _taskItem.SetMetadata(nameof(Filter), value);
    }

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public string Path {
        get => _taskItem.GetMetadata(nameof(Path));
        set => _taskItem.SetMetadata(nameof(Path), value);
    }

    /// <summary>
    /// The root local repository cache folder.
    /// </summary>
    public string RepositoryRoot
    {
        get => _taskItem.GetMetadata(nameof(RepositoryRoot));
        set => _taskItem.SetMetadata(nameof(RepositoryRoot), value);
    }

    /// <summary>
    /// true when the target already exists, and has the expected file(s)
    /// </summary>
    public string TargetExists 
    {
        get => _taskItem.GetMetadata(nameof(TargetExists));
        set => _taskItem.SetMetadata(nameof(TargetExists), value);
    }
}