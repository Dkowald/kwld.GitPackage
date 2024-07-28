using GitGet.Model;
using GitGet.Utility;

using GitPackage.Cli.Model;

using LibGit2Sharp;
using LibGit2Sharp.Handlers;

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
        var (package, _) = await StatusFile.LoadWithArgumentOverides(_log, args);

        if (package is null) 
        { return 1; }

        _log.LogInformation("GitPackage restore for '{outPath}'", package.TargetPath);
        _log.LogDebug("  Repo: {origin}", package.Origin);
        _log.LogDebug("  Ver: {version}", package.Version);
        _log.LogDebug("  Filter: {filter}", package.Filter);

        _log.LogDebug("  User: {user}", args.User ?? "");
        _log.LogDebug("  Password: {pwd}", args.HasPassword ? "****" : "");

        var force = IsForce(args, package.Version);
        if (force)
        {
            package.Commit = null;
            _log.LogDebug("Force refresh from server."); 
        }

        if (!package.Commit.IsNullOrEmpty())
        {
            _log.LogInformation("GitPackage commit exist; no work to do");
            return 0;
        }

        //Creds.
        CredentialsHandler? creds = args.HasPassword?
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
        if (targetRef is null)
        {
            _log.LogError("Unable to resolve git ref {gitRef}", package.Version.Version);
            return 1;
        }

        //reset out folder.
        _log.LogInformation("Clean {TargetPath}", args.TargetPath.FullName);
        CleanTargetPath(args.TargetPath);

        _log.LogInformation("Extracting files");
        var info = await new GitCommands.Get(repo)
            .Run(package.TargetPath, package.Version, package.Filter);

        package.Commit = info.Commit;

        _log.LogInformation("Found {count} files", info.Extracted);
        
        await package.Write(_log);

        return 0;
    }

    private DirectReference? FetchReference(Repository repo, GitRef gitRef, bool force, CredentialsHandler? creds)
    {
        if (gitRef.IsTag && !force)
        {
            var tagRef = repo.Refs[gitRef];
            if(tagRef != null)
            {
                _log.LogInformation("Ref '{gitRef}' found in cache", gitRef.Version);
                return tagRef.ResolveToDirectReference();
            }
            else
            {
                _log.LogDebug("Tag {gitRef} not found in cache", gitRef.Version);
            }
        }

        _log.LogInformation("Fetching Ref '{gitRef}' from server", gitRef.Version);

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
            CredentialsProvider = creds,
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
        
        var targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        return targetRef;
    }

    private static void CleanTargetPath(IDirectoryInfo targetPath)
    {
        if (!targetPath.Exists()) return;

        foreach(var item in targetPath.EnumerateFileSystemInfos())
        {
            if(item is IDirectoryInfo dir)
            {
                try
                {
                    dir.Delete(true);
                }catch(Exception ex)
                {
                    throw new Exception($"Failed delete {dir.FullName}", ex);
                }
            }

            if(item is IFileInfo file)
            {
                if (file.Name == StatusFile.FileName && file.DirectoryName == targetPath.FullName)
                    continue;

                try
                {
                    file.Delete();
                }catch(Exception ex)
                {
                    throw new Exception($"Filed delete {file.FullName}", ex);
                }
            }
        }
    }

    private bool IsForce(Args args, GitRef targetRef)
    {
        if (args.Force == ForceOption.All) return true;

        if(targetRef.IsBranch && args.Force == ForceOption.Branch)
        {
            _log.LogInformation("Forcing branch re-get");
            return true;
        }

        if (targetRef.IsTag && args.Force == ForceOption.Tag)
        {
            _log.LogInformation("Forcing tag re-get");
            return true;
        }

        return false;
    }
}