using GitGet.Utility;

using Microsoft.Extensions.Logging;

namespace GitGet.Model;

/// <summary>
/// Match a source repository URL to a local Folder.
/// </summary>
internal class RepositoryCache
{
    public static readonly string DefaultCacheFolderName = ".gitpackages";

    private readonly ILogger _appLog;
    private readonly IDirectoryInfo _cacheRoot;

    public RepositoryCache(ILogger appLog, IFileSystem files, IDirectoryInfo? customRoot)
    {
        _appLog = appLog;

        if (customRoot is not null)
        {
            appLog.LogInformation("Using custom repository cache");
            _cacheRoot = customRoot;
        }
        else
        {
            _cacheRoot = ResolveCache(files);
        }


        appLog.LogDebug("Using repository cache at {RepositoryCache}", _cacheRoot.FullName);
    }

    public CacheEntry Get(Uri origin)
    {
        if (origin.IsFile)
        {
            _appLog.LogInformation("Repository is Local folder, using 'local' as cache host name");

            var localRepo = _cacheRoot.FileSystem.DirectoryInfo.New(origin.LocalPath);

            return new CacheEntry(origin, _cacheRoot.GetFolder("local", localRepo.Name));
        }

        var relPath = $"{origin.Host}{origin.AbsolutePath}".Replace('\\', '/');
        var path = _cacheRoot.GetFolder(relPath);
        return new(origin, path);
    }

    public class CacheEntry(Uri origin, IDirectoryInfo cachePath)
    {
        public Uri Origin => origin;

        public IDirectoryInfo CachePath => cachePath;
    }

    private IDirectoryInfo ResolveCache(IFileSystem files)
    {
        var home = files.TryGetHome();

        if (home is null)
        {
            _appLog.LogWarning("No home directory found!, using current directory");
        }

        home ??= files.Current();

        var cache = home.GetFolder(DefaultCacheFolderName);

        return cache;
    }
}
