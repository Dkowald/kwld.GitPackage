using GitPackage.Cli.GitCommands;
using GitPackage.Cli.Model;
using LibGit2Sharp;

namespace GitPackage.Cli;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        if(!args.Any()) Help();

        throw new NotImplementedException();
    }

    public static async Task<int> Run(AppConfig config, GitPackageItem item)
    {
        var files = new FileSystem();

        var target = files.DirectoryInfo.New(item.Path);

        var cacheRoot = files.DirectoryInfo.New(config.RepositoryCache)
            .EnsureExists();

        var cache = new RepositoryCache(cacheRoot, item.Include);

        var status = new GitGetStatus(target, item.Include, item.Version);

        if (status.IsMatch()) {return 0;}

        if (!cache.CachePath.Exists)
        {
            cache.CachePath.EnsureExists();
            Repository.Clone(cache.Origin.ToString(), cache.CachePath.FullName,
                new CloneOptions
                {
                    IsBare = true,
                });
        }

        var repo = new Repository(cache.CachePath.FullName);

        var gitRef = repo.Refs[item.Version.Value];
        if (gitRef is null)
        {
            var remote = repo.Network.Remotes["origin"];
            var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);

            Commands.Fetch(repo, "origin", refSpecs, new()
            { TagFetchMode = TagFetchMode.All}, "");

            gitRef = repo.Refs[item.Version.Value];
        }
        
        if (gitRef is null)
            throw new Exception($"Commit ref not found: {item.Version}");

        target.EnsureEmpty();

        new GitGet(repo)
            .Run(target, item.Version, new(item.Filter));

        await status.SetMatched();

        return 0;
    }

    static void Version(){}

    static void Help()
    {
        Console.WriteLine("GitPackage CLI");
        
        Console.WriteLine("GitPackage CLI");
    }
}