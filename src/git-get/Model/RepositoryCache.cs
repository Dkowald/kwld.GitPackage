using LibGit2Sharp;
using LibGit2Sharp.Handlers;

using Microsoft.Extensions.Logging;

namespace GitGet.Model;

/// <summary>
/// Match a source repository URL to a local Folder.
/// </summary>
internal class RepositoryCache
{
    public static readonly string DefaultCacheFolderName = ".gitpackages";
    public static readonly string LocalHostPath = "local";

    private readonly ILogger _log;
    private readonly IDirectoryInfo _cacheRoot;

    public RepositoryCache(ILogger log, IDirectoryInfo cacheRoot)
    {
        _log = log;
        _cacheRoot = cacheRoot;

        log.LogTrace("Using repository cache at {RepositoryCache}",
            _cacheRoot.FullName);
    }

    public class CacheEntry(Uri origin, IDirectoryInfo cachePath)
    {
        public Uri Origin => origin;

        public IDirectoryInfo CachePath => cachePath;
    }

    public CacheEntry Get(Uri origin)
    {
        if(origin.IsFile) {
            _log.LogDebug("Repository is Local folder, using 'local' as cache host name");

            var localRepo = _cacheRoot.FileSystem.DirectoryInfo.New(origin.LocalPath);

            return new CacheEntry(origin, _cacheRoot.GetFolder(LocalHostPath, localRepo.Name));
        }

        var relPath = $"{origin.Host}{origin.AbsolutePath}".Replace('\\', '/');
        var path = _cacheRoot.GetFolder(relPath);
        return new(origin, path);
    }

    public IEnumerable<CacheEntry> List()
    {
        var repoPaths = _cacheRoot
            .GetDirectories("refs", SearchOption.AllDirectories)
            .Select(x => x.Parent!);

        var entries = repoPaths.Select(TryResolveEntry)
            .Where(x => x is not null)
            .Select(x => x!);

        return entries;
    }

    /// <inheritdoc cref="CloneIfMissing(CacheEntry,LibGit2Sharp.Handlers.CredentialsHandler?)" />
    public Repository CloneIfMissing(Uri origin, CredentialsHandler? creds)
        => CloneIfMissing(Get(origin), creds);

    /// <summary>
    /// Ensures git repository is in the cache,
    /// using credentials for clone if provided.
    /// </summary>
    /// <param name="cache">Cache entry corresponding to location for the origin</param>
    /// <param name="creds">Credentials to use when cloning (if needed)</param>
    public Repository CloneIfMissing(CacheEntry cache, CredentialsHandler? creds)
    {
        if(Repository.IsValid(cache.CachePath.FullName)) {
            _log.LogTrace("Cached repository {origin} found", cache.Origin);
            return new Repository(cache.CachePath.FullName);
        }

        CloneWithLock(cache, creds);

        var repo = new Repository(cache.CachePath.FullName);

        return repo;
    }

    public RepositoryCache Purge(Uri origin)
    {
        var entry = Get(origin);
        if(entry.CachePath.Exists()) {
            entry.CachePath.EnsureDelete();
        }
        return this;
    }

    private void CloneWithLock(CacheEntry cache, CredentialsHandler? creds)
    {
        if(cache.CachePath.Exists) {
            _log.LogWarning("Cached repository broken, resetting {Origin}", cache.Origin);
            cache.CachePath.EnsureDelete();
        }
        _log.LogInformation("Cloning source repository '{origin}'", cache.Origin);

        cache.CachePath.EnsureExists();

        var options = new CloneOptions {
            IsBare = true,
            FetchOptions =
            {
                    CredentialsProvider = creds,
                    TagFetchMode = TagFetchMode.Auto
                }
        };

        var progressStarted = false;
        options.FetchOptions.OnProgress += _ => {
            if(!progressStarted) {
                _log.LogDebug("Fetching objects to transfer");
                progressStarted = true;
            }

            return true;
        };

        var transferStarted = false;
        options.FetchOptions.OnTransferProgress = x => {
            if(!transferStarted) {
                _log.LogDebug("Fetching {totalObjects} from server", x.TotalObjects);
                transferStarted = true;
            }

            return true;
        };

        Repository.Clone(cache.Origin.ToString(), cache.CachePath.FullName, options);

    }

    private CacheEntry? TryResolveEntry(IDirectoryInfo repoPath)
    {
        if(!Repository.IsValid(repoPath.FullName)) {
            _log.LogWarning("Cache folder {cache} is NOT a valid repository",
                repoPath.FullName);
            return null;
        }

        using var config = new Repository(repoPath.FullName).Config;

        var origin = config
            .Get<string>("remote.origin.url", ConfigurationLevel.Local)
            ?.Value;
        if(origin is null) {
            _log.LogError("Repository cache {cache} does not have origin remote", repoPath.FullName);
            return null;
        }

        if(!Uri.TryCreate(origin, UriKind.Absolute, out var originUri)) {
            _log.LogError("Cache {cache} origin url is not a uri",
                repoPath.FullName);
            return null;
        }

        var entry = new CacheEntry(originUri, repoPath);

        return entry;
    }
}
