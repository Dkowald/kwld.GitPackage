using GitGet.Model;
using GitGet.Utility;

using GitPackage.Cli.Model;

using LibGit2Sharp;

using Microsoft.Extensions.Logging;

namespace GitGet.Actions;

internal class Get : IAction
{
    private readonly ILogger _log;
    
    public Get(ILogger log)
    {
        _log = log;
    }

    public async Task<int> Run(Args args)
    {
        var package = TryBuildStatusFile(args);

        if (package is null) return 1;

        _log.LogInformation("Package sync for '{outPath}'", package.TargetPath);
        _log.LogDebug("  Repo: {origin}", package.Origin);
        _log.LogDebug("  Ver: {version}", package.Version);
        _log.LogDebug("  Filter: {filter}", package.Filter);

        if (!package.Commit.IsNullOrEmpty())
        {
            _log.LogInformation("Package commit exist; no work to do");
            return 0;
        }

        //Clone
        var cache = new RepositoryCache(_log, args.Cache);
        var entry = cache.Get(package.Origin);
        var repo = cache.CloneIfMissing(entry);

        //Check for ref.
        var targetRef = FetchReference(repo, package.Version);
        if (targetRef is null)
        {
            _log.LogError("Unable to resolve git ref {gitRef}", package.Version.Version);
            return 1;
        }

        //Check if already have.
        var commit = targetRef.Target.Peel<Commit>();
        if (package.Commit == commit.Sha)
        {
            _log.LogInformation("Required commit already extracted");
            return 0;
        }

        //reset out folder.
        package.TargetPath.EnsureEmptyWithoutDelete();
        package.Write(_log);

        new GitCommands.Get(repo)
            .Run(package.TargetPath, package.Version, package.Filter);

        package.Commit = commit.Sha;
        package.Write(_log);

        return 0;
    }

    private StatusFile? TryBuildStatusFile(Args args)
    {
        var package = StatusFile.LoadIfFound(_log, args.TargetPath);

        if (package is null) package = TryCreatePackageFromArgs(args);
        else AssignArgsToPackage(args, package);

        return package;
    }

    private StatusFile? TryCreatePackageFromArgs(Args args)
    {
        if (args.Origin is null)
        {
            _log.LogError("Origin not provided");
            return null;
        }

        if (args.Version is null)
        {
            _log.LogError("Version not provided");
            return null;
        }

        var filter = args.Filter;
        if (filter is null)
        {
            _log.LogInformation("Using default select-all filter");
            filter = new();
        }

        return new(args.TargetPath, args.Origin, args.Version, filter);
    }

    private Repository CloneIfMissing(RepositoryCache.CacheEntry cache)
    {
        if (!cache.CachePath.Exists)
        {
            _log.LogInformation("Cloning source repository '{origin}'", cache.Origin);

            cache.CachePath.EnsureExists();
            Repository.Clone(cache.Origin.ToString(), cache.CachePath.FullName,
                new CloneOptions { IsBare = true });
        }
        else
        {
            _log.LogDebug("Cached repository {origin} found", cache.Origin);
        }

        var repo = new Repository(cache.CachePath.FullName);

        return repo;
    }

    private DirectReference? FetchReference(Repository repo, GitRef gitRef)
    {
        //[24] = refs / remotes / origin / master => "5085a0c6173cdb2a3fde205330b327a8eb0a26c4"
        var targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        if (targetRef is null)
        {
            _log.LogInformation("Ref '{gitRef}', not found, fetching data from server", gitRef.Version);

            //fetch.
            var refSpecs = repo.Network.Remotes["origin"].FetchRefSpecs.Select(x => x.Specification);

            var progress = new List<string>();
            var transfer = new List<string>();
            var progressStarted = false;
            var transferStarted = false;

            var options = new FetchOptions()
            {
                TagFetchMode = TagFetchMode.All,
                Prune = true,
                OnProgress = txt =>
                {
                    if (!progressStarted)
                    {
                        //todo: match with clone reporting.
                        _log.LogDebug("Fetching objects to transfer");
                        progressStarted = true;
                    }
                    progress.Add(txt);
                    return true;
                },
                OnTransferProgress = x => {
                    if (!transferStarted)
                    {
                        _log.LogDebug("Fetching {totalObjects} from server", x.TotalObjects);
                        transferStarted = true;
                    }
                    return true; 
                }
            };

            Commands.Fetch(repo, "origin", refSpecs, options, "");
        }
        else
        {
            _log.LogDebug("Cached repository contains target ref: {gitRef}", gitRef.Version);
        }

        targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        return targetRef;
    }

    private void AssignArgsToPackage(Args args, StatusFile package)
    {
        var hasChanged = false;
        if (args.Origin is not null && args.Origin != package.Origin)
        {
            _log.LogInformation("Updating package origin");

            package.Origin = args.Origin;
            hasChanged = true;
        }

        if (args.Version is not null && args.Version != package.Version)
        {
            _log.LogInformation("Updating package version");
            package.Version = args.Version;
            hasChanged = true;
        }

        if (args.Filter is not null && args.Filter != package.Filter)
        {
            _log.LogInformation("Updating package file filter");
            package.Filter = args.Filter;
            hasChanged = true;
        }

        if (args.Force is not null)
        {
            if (args.Force == ForceOption.All ||
                (args.Force == ForceOption.Branch && package.Version.IsBranch) ||
                (args.Force == ForceOption.Tag && package.Version.IsTag)
               )
            {
                _log.LogInformation("Forcing re-evaluate get files.");
                hasChanged = true;
            }
        }

        if (hasChanged)
            package.Commit = null;
    }
}