namespace GitPackage.Cli.Model;

/// <summary>
/// Match a source repository URL to a local Folder.
/// </summary>
internal class RepositoryCache
{
    public RepositoryCache(IDirectoryInfo cacheRoot, string originUrl)
    {
        if (!Uri.TryCreate(originUrl, UriKind.Absolute, out var origin))
            throw new Exception("Repository origin must be URl");

        Origin = origin;

        if (origin.IsFile)
        {
            var localRepo = cacheRoot.FileSystem.DirectoryInfo.New(originUrl);
            CachePath = cacheRoot.GetFolder("local", localRepo.Name);
        }
        else
        {
            var relPath = $"{origin.Host}{origin.AbsolutePath}".Replace('\\', '/');
            CachePath = cacheRoot.GetFolder(relPath);
        }
    }

    public Uri Origin { get; }

    public IDirectoryInfo CachePath { get; }
}