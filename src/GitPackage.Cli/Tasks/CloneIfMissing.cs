using GitPackage.Cli.Model;
using LibGit2Sharp;

namespace GitPackage.Cli.Tasks;

internal class CloneIfMissing
{
    private readonly RepositoryCache _cache;
    private readonly GitPackageItem _item;
    
    public CloneIfMissing(RepositoryCache cache, GitPackageItem item)
    {
        _cache = cache;
        _item = item;
    }
    
    public void Run()
    {
        if(_cache.CachePath.Exists)return;

        _cache.CachePath.EnsureExists();

        Repository.Clone(_cache.Origin.ToString(), _cache.CachePath.FullName,
            new CloneOptions{ IsBare = true });
    }

    public Repository Repository =>
        new (_cache.CachePath.FullName);
}
