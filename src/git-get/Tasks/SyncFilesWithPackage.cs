using GitPackage.Cli.GitCommands;
using GitPackage.Cli.Model;

using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Tasks;

internal class SyncFilesWithPackage
{
    private readonly ILogger _appLog;

    public SyncFilesWithPackage(ILogger appLog)
    {
        _appLog = appLog;
    }

    public async Task<int> Run(AppConfig config)
    {
        var package = config.Package;

        if (package is null || config.OutDir is null)
        {
            _appLog.LogError("Unable to read git get package details");
            return 1;
        }

        if (package.Include is null)
        {
            _appLog.LogError("Missing repository for git get package");
            return 1;
        }

        if (package.Version is null)
        {
            _appLog.LogError("Missing target branch / tag for git get package");
            return 1;
        }

        _appLog.LogInformation("Package sync for '{outPath}'", config.OutDir.Name);
        _appLog.LogDebug("  Repo: {origin}", package.Include);
        _appLog.LogDebug("  Ver: {version}", package.Version);
        _appLog.LogDebug("  Ver: {filter}", package.Filter);

        if (!package.Commit.IsNullOrEmpty())
        {
            _appLog.LogInformation("Package commit exist; no work to do");
            return 0;
        }

        var cache = RepositoryCache.New(_appLog, config.RepositoryCache, package.Include);
        if (cache is null) return 1;

        //Clone
        var repo = CloneIfMissing(cache);

        //Check for ref.
        var targetRef = FetchReference(repo, package.Version);
        if (targetRef is null)
        {
            _appLog.LogError("Unable to resolve git ref {gitRef}", package.Version.Version);
            return 1;
        }

        //Check if already have.
        var commit = targetRef.Target.Peel<Commit>();

        if (package.Commit == commit.Sha)
        {
            return 0;
        }

        new GitGet(repo)
            .Run(config.OutDir, package.Version, package.Filter);

        package.Commit = commit.Sha;
        package.Write();

        return 0;
    }

    private Repository CloneIfMissing(RepositoryCache cache)
    {
        if (!cache.CachePath.Exists)
        {
            _appLog.LogInformation("Cloning source repository '{origin}'", cache.Origin);

            cache.CachePath.EnsureExists();
            Repository.Clone(cache.Origin.ToString(), cache.CachePath.FullName,
                new CloneOptions { IsBare = true });
        }
        else
        {
            _appLog.LogDebug("Cached repository {origin} found", cache.Origin);
        }

        var repo = new Repository(cache.CachePath.FullName);

        return repo;
    }

    private DirectReference? FetchReference(Repository repo, GitRef gitRef)
    {
        var targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        if (targetRef is null)
        {
            _appLog.LogInformation("Ref '{gitRef}', not found, refreshing data from server", gitRef.Version);

            //fetch.
            var refSpecs = repo.Network.Remotes["origin"].FetchRefSpecs.Select(x => x.Specification);
            Commands.Fetch(repo, "origin", refSpecs, new() { TagFetchMode = TagFetchMode.All }, "");
        }
        else
        {
            _appLog.LogDebug("Cached repository contains target ref: {gitRef}", gitRef.Version);
        }

        targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        return targetRef;
    }
}