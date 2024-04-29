namespace GitPackage.Model;

/// <summary>
/// Match a source repository URL to a local Folder.
/// </summary>
internal class SourceRepository
{
    public SourceRepository(IDirectoryInfo localRoot, string originUrl)
    {   
        if (!Uri.TryCreate(originUrl, UriKind.Absolute, out var origin))
        {
            throw new Exception("todo: allow simplified url with assume github");
        }

        Origin = origin;

        var relPath = $"{origin.Host}{origin.AbsolutePath}".Replace('\\','/');

        LocalPath = localRoot.GetFolder(relPath);
    }

    public Uri Origin { get; }

    public IDirectoryInfo LocalPath { get; }
}