using GitGet.Model;
using GitPackage.Cli.Model;

using LibGit2Sharp;
using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Tasks;

internal class Get
{
    private readonly ILogger _appLog;
    private readonly RepositoryCache _cache;

    public Get(ILogger appLog, RepositoryCache cache)
    {
        _appLog = appLog;
        _cache = cache;
    }

    public async Task<int> Run(Args args)
    {
        var package = GitPackageStatusFile.LoadIfFound(_appLog, args.TargetPath)
            ?? new GitPackageStatusFile(_appLog, args.TargetPath);

        MergeArgs(args, package);

        if (package.Origin is null)
        {
            _appLog.LogError("Missing repository for git get package");
            return 1;
        }

        if (package.Version is null)
        {
            _appLog.LogError("Missing target branch / tag for git get package");
            return 1;
        }

        _appLog.LogInformation("Package sync for '{outPath}'", args.TargetPath.Name);
        _appLog.LogDebug("  Repo: {origin}", package.Origin);
        _appLog.LogDebug("  Ver: {version}", package.Version);
        _appLog.LogDebug("  Filter: {filter}", package.Filter);

        if (!package.Commit.IsNullOrEmpty())
        {
            _appLog.LogInformation("Package commit exist; no work to do");
            return 0;
        }

        var cache = _cache.Get(new(package.Origin));

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

        new GitGet.GitCommands.Get(repo)
            .Run(args.TargetPath, package.Version, package.Filter);

        package.Commit = commit.Sha;
        package.Write();

        return 0;
    }

    [Obsolete]
    public async Task<int> Run(AppConfig config)
    {
        var package = config.Package;

        if (package is null || config.OutDir is null)
        {
            _appLog.LogError("Unable to read git get package details");
            return 1;
        }

        if (package.Origin is null)
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
        _appLog.LogDebug("  Repo: {origin}", package.Origin);
        _appLog.LogDebug("  Ver: {version}", package.Version);
        _appLog.LogDebug("  Ver: {filter}", package.Filter);

        if (!package.Commit.IsNullOrEmpty())
        {
            _appLog.LogInformation("Package commit exist; no work to do");
            return 0;
        }

        var cache = _cache.Get(new(package.Origin));
        
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

        new GitGet.GitCommands.Get(repo)
            .Run(config.OutDir, package.Version, package.Filter);

        package.Commit = commit.Sha;
        package.Write();

        return 0;
    }

    private Repository CloneIfMissing(RepositoryCache.CacheEntry cache)
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

    private void MergeArgs(Args args, GitPackageStatusFile package)
    {
        if (args.Origin is not null && args.Origin.ToString() != package.Origin)
        {
            _appLog.LogInformation("Updating package origin");

            package.Origin = args.Origin.ToString();
            package.Commit = null;
        }

        if (args.Version is not null && args.Version != package.Version)
        {
            _appLog.LogInformation("Updating package version");
            package.Version = args.Version;
            package.Commit = null;
        }

        if (args.Filter is not null && args.Filter != package.Filter)
        {
            _appLog.LogInformation("Updating package file filter");
            package.Filter = args.Filter;
            package.Commit = null;
        }
    }
}