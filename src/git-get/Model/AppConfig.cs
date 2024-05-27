using GitPackage.Cli.Utility;
using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Model;

internal class AppConfig
{
    public static readonly string DefaultCacheFolderName = ".gitpackages";

    private readonly ILogger _appLogger;
    private readonly IFileSystem _files;
    private readonly Config _args;

    public AppConfig(ILogger appLogger, IFileSystem files, Config args)
    {
        _appLogger = appLogger;
        _files = files;
        _args = args;

        var statusFile = ResolveStatusFile();

        Package = statusFile?.Exists == true ? 
            GitPackageStatusFile.Load(appLogger, statusFile) : null;

        OutDir = statusFile?.Directory;

        RepositoryCache = ResolveCache();
    }

    public IDirectoryInfo RepositoryCache { get; private set; }

    public GitPackageStatusFile? Package { get; private set; }

    public IDirectoryInfo? OutDir { get; private set; }

    private IFileInfo? ResolveStatusFile()
    {
        if (_args.DataFile is null)
        {
            //todo: allow to describe by just cli args.
            return null;
        }

        var statusFile = _files.Current().GetFile(_args.DataFile);
        if (!statusFile.Exists)
        {
            _appLogger.LogError("Missing StatusFile: {StatusFile}", statusFile.FullName);
            return null;
        }

        return statusFile;
    }
    
    private IDirectoryInfo ResolveCache()
    {
        if (_args.Cache is not null)
        {
            var custom = _files.Current().GetFolder(_args.Cache);
            _appLogger.LogDebug("Using custom repository cache: '{cache}'", custom.FullName);
            return custom;
        }

        var home = _files.TryGetHome();

        if (home is null)
        {
            _appLogger.LogWarning("No home directory found!, using cwd");
        }
            
        home ??= _files.Current();

        var cache = home.GetFolder(DefaultCacheFolderName);

        _appLogger.LogDebug("Cache path: {cache}", cache.FullName);

        return cache;
    }
}