using GitGet.Model;

using Microsoft.Extensions.Logging;

namespace GitGet.Actions;

internal class Init : IAction
{
    private readonly ILogger _log;
    public Init(ILogger log)
    {
        _log = log;
    }

    public async Task<int> Run(Args args)
    {
        var cache = new RepositoryCache(_log, args.Cache);

        var package = await StatusFile.TryLoad(_log, args.TargetPath);
        
        var changed = false;

        if (package is null)
        {
            if(args.Origin is null)
            {
                _log.LogError("Repository origin missing");
                return 1;
            }
            if(args.Version is null)
            {
                _log.LogError("Missing version");
                return 1;
            }

            var filter = args.Filter ?? new();

            package = new(args.TargetPath, args.Origin, args.Version, filter);
            _log.LogDebug("Create new package status file {GitPackageStatusFile}",
                package.TargetPath.FullName);
            changed = true;
        }
        else
        {
            if(args.Origin != null && package.Origin != args.Origin)
            {
                changed = true;
                package.Origin = args.Origin;
                _log.LogDebug("Updating package origin");
            }

            if(args.Filter != null && package.Filter != args.Filter)
            {
                changed = true;
                package.Filter = args.Filter;
                _log.LogDebug($"Updating package {nameof(StatusFile.Filter)}");
            }

            if(args.Version != null && package.Version != args.Version)
            {
                changed = true;
                package.Version = args.Version;
                _log.LogDebug("Updating package version");
            }
        }

        if (changed)
        {
            _log.LogInformation("Writing updated package file");
            await package.Write(_log);
        }

        return 0;
    }
}