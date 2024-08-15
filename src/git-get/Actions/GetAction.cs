using GitGet.Actions.Errors;
using GitGet.Model;
using GitGet.Utility;

using LibGit2Sharp;
using LibGit2Sharp.Handlers;

using Microsoft.Extensions.Logging;

namespace GitGet.Actions;

internal class GetAction : IAction
{
    private readonly ILogger _log;

    public GetAction(ILogger log)
    {
        _log = log;
    }

    public async Task<int> Run(Args args)
    {
        var (package, _) = await StatusFile.TryLoadWithOverrides(_log, args);

        if(package is null) { return 1; }

        _log.LogInformation("GitPackage restore for '{outPath}'", package.TargetPath);
        _log.LogDebug("  Repo: {origin}", package.Origin);
        _log.LogDebug("  Ver: {version}", package.Version);
        _log.LogDebug("  Filter: {filter}", package.Filter);
        _log.LogDebug("  Ignore: {ignore}", package.Ignore);
        _log.LogDebug("  GetRoot: {get-root}", package.GetRoot);

        _log.LogDebug("  User: {user}", args.User ?? "");
        _log.LogDebug("  Password: {pwd}", args.HasPassword ? "****" : "");

        var force = IsForce(args, package.Version);
        if(force) {
            package.Commit = null;
            _log.LogDebug("Force refresh from server.");
        }

        if(!package.Commit.IsNullOrEmpty()) {
            _log.LogInformation("GitPackage commit exist; no work to do");
            return 0;
        }

        //Creds.
        CredentialsHandler? creds = args.HasPassword ?
            (_, _, _) => new UsernamePasswordCredentials {
                Username = args.User,
                Password = args.UsePassword()
            } : null;

        //Clone
        var cache = new RepositoryCache(_log, args.Cache);
        var entry = cache.Get(package.Origin);
        using var repo = cache.CloneIfMissing(entry, creds);

        //Check for ref.
        var targetRef = FetchReference(repo, package.Version, force, creds);
        if(targetRef is null) {
            _log.LogError("Unable to resolve git ref {gitRef}", package.Version.Version);
            return 1;
        }

        ResetOutputPath(args.TargetPath);

        _log.LogInformation("Extracting files");
        var info = await new GitCommands.Get(repo)
            .Run(package.TargetPath, package.Version, package.Filter, package.Ignore, package.GetRoot);

        package.Commit = info.Commit;

        _log.LogInformation("Found {count} files, Matched {included}, Ignored {ignored}",
            info.TotalItems, info.IncludedItemsCount, info.IgnoredItemsCount);

        if(package.TargetPath.GetFile(StatusFile.FileName).Exists()) {
            _log.LogWarning($"Extracted file {StatusFile.FileName} is being overwritten with status file data.");
        }

        await package.Write(_log);

        return 0;
    }

    private void ResetOutputPath(IDirectoryInfo targetPath)
    {
        //reset out folder.
        _log.LogInformation("Clean {TargetPath}", targetPath.FullName);

        try {
            targetPath.MakeEmpty();
        } catch(Exception ex) {
            throw new ErrorCannotCleanTarget(targetPath.FullName, ex);
        }
    }

    private DirectReference? FetchReference(Repository repo, GitRef gitRef, bool force, CredentialsHandler? creds)
    {
        if(gitRef.IsTag && !force) {
            var tagRef = repo.Refs[gitRef.Value];
            if(tagRef != null) {
                _log.LogInformation("Ref '{gitRef}' found in cache", gitRef.Version);
                return tagRef.ResolveToDirectReference();
            }

            _log.LogDebug("Tag {gitRef} not found in cache", gitRef.Version);
        }

        _log.LogInformation("Fetching Ref '{gitRef}' from server", gitRef.Version);

        //fetch.
        var remote = repo.Network.Remotes["origin"];
        var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification).ToArray();

        var progressStarted = false;
        var transferStarted = false;

        var options = new FetchOptions {
            TagFetchMode = TagFetchMode.All,
            Prune = true,
            CredentialsProvider = creds,
            OnProgress = _ => {
                if(!progressStarted) {
                    //todo: match with clone reporting.
                    _log.LogDebug("Fetching objects to transfer");
                    progressStarted = true;
                }
                return true;
            },
            OnTransferProgress = x => {
                if(!transferStarted) {
                    _log.LogDebug("Fetching {totalObjects} from server", x.TotalObjects);
                    transferStarted = true;
                }
                return true;
            }
        };

        Commands.Fetch(repo, "origin", refSpecs, options, "");

        var targetRef = repo.Refs[gitRef.Value]?.ResolveToDirectReference();

        return targetRef;
    }

    private bool IsForce(Args args, GitRef targetRef)
    {
        if(args.Force == ForceOption.All) return true;

        if(targetRef.IsBranch && args.Force == ForceOption.Branch) {
            _log.LogInformation("Forcing branch re-get");
            return true;
        }

        if(targetRef.IsTag && args.Force == ForceOption.Tag) {
            _log.LogInformation("Forcing tag re-get");
            return true;
        }

        return false;
    }
}