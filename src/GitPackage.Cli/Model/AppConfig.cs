using GitPackage.Cli.Utility;

namespace GitPackage.Cli.Model;

internal class AppConfig
{
    public static readonly string DefaultCacheFolderName = ".gitpackages";

    private readonly IFileSystem _files;
    private readonly Args _args;

    public AppConfig(IFileSystem files, Args args)
    {
        _files = files;
        _args = args;

        var statusFile = StatusFile();

        Package = statusFile?.Exists == true ? GitPackageStatusFile.Load(statusFile) : null;

        OutDir = statusFile?.Directory;
    }

    public IDirectoryInfo RepositoryCache()
    {
        if (_args.Cache is not null)
        {
            return _files.Current().GetFolder(_args.Cache);
        }

        var home = _files.TryGetHome() ?? _files.Current();

        return home.GetFolder(DefaultCacheFolderName);
    }

    public GitPackageStatusFile? Package { get; private set; }

    public IDirectoryInfo? OutDir { get; private set; }

    private IFileInfo? StatusFile()
    {
        if (_args.DataFile is null)
        {
            //todo: allow describe by just cli args.
            return null;
        }

        var statusFile = _files.Current().GetFile(_args.DataFile);
        if (!statusFile.Exists)
        {
            //todo: report error not exist.
            return null;
        }

        return statusFile;
    }
}