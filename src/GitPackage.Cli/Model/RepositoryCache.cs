using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Model;

/// <summary>
/// Match a source repository URL to a local Folder.
/// </summary>
internal class RepositoryCache
{
    public static RepositoryCache? New(ILogger appLog, IDirectoryInfo cacheRoot, string? originUrl)
    {
        if (originUrl is null)
        {
            appLog.LogError("Repository URL cannot be empty");
            return null;
        }

        if (!Uri.TryCreate(originUrl, UriKind.Absolute, out var origin))
        {
            appLog.LogError("Repository origin must be URl: {PackageUrl}", originUrl);
            return null;
        }

        return new RepositoryCache(appLog, cacheRoot, origin);
    }

    public RepositoryCache(ILogger appLog, IDirectoryInfo cacheRoot, Uri origin)
    {
        Origin = origin;

        if (origin.IsFile)
        {
            appLog.LogInformation("Repository is Local folder, using 'local' as cache host name");

            var localRepo = cacheRoot.FileSystem.DirectoryInfo.New(origin.LocalPath);
            CachePath = cacheRoot.GetFolder("local", localRepo.Name);
        }
        else
        {
            var relPath = $"{origin.Host}{origin.AbsolutePath}".Replace('\\', '/');
            CachePath = cacheRoot.GetFolder(relPath);
        }

        appLog.LogInformation("Using repository cache at {RepositoryCache}", CachePath.FullName);
    }

    public Uri Origin { get; }

    public IDirectoryInfo CachePath { get; }
}