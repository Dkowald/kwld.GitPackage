using GitPackage.Cli.GitCommands;
using GitPackage.Cli.Model;
using GitPackage.Cli.Tasks;
using LibGit2Sharp;

namespace GitPackage.Cli;

internal class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var files = new FileSystem();

        var argInfo = new Args(args);

        if(argInfo.ShowVersion) {
            Version();
            return 0;
        }

        var config = new AppConfig(files, argInfo);

        var result = await new SyncFilesWithPackage().Run(config);

        return result;
    }

    //public static async Task<int> Run(AppConfig config)
    //{
    //    var package = config.GetPackage();
    //    if (package is null) 
    //        throw new Exception("No package defined");
        
    //    var cache = new RepositoryCache(config.RepositoryCache(), package.Include);

    //    //Clone
    //    var repo = CloneIfMissing(cache);

    //    //Check for ref.
    //    var targetRef = FetchReference(repo, package.Version)
    //        ?? throw new Exception($"Cannot find {package.Version}");

    //    //Check if already have.
    //    var commit = targetRef.Target.Peel<Commit>();

    //    if (package.Commit == commit.Sha)
    //    {
    //        return 0;
    //    }

    //    new GitGet(repo)
    //        .Run(config.OutFolder(), package.Version, new(package.Filter));

    //    package.Commit = commit.Sha;
    //    package.Write();

    //    return 0;
    //}

    private static Repository CloneIfMissing(RepositoryCache cache)
    {
        if (!cache.CachePath.Exists)
        {
            cache.CachePath.EnsureExists();
            Repository.Clone(cache.Origin.ToString(), cache.CachePath.FullName,
                new CloneOptions { IsBare = true });
        }

        var repo = new Repository(cache.CachePath.FullName);

        return repo;
    }

    private static DirectReference? FetchReference(Repository repo, GitRef gitRef)
    {
        var targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        if (targetRef is null)
        {
            //fetch.
            var refSpecs = repo.Network.Remotes["origin"].FetchRefSpecs.Select(x => x.Specification);
            Commands.Fetch(repo, "origin", refSpecs, new() { TagFetchMode = TagFetchMode.All }, "");
        }

        targetRef = repo.Refs[gitRef]?.ResolveToDirectReference();

        return targetRef;
    }

    static void Version()
    {
        Console.WriteLine("Version 0.0.1");
    }

    static void EchoArgs(string[] args)
    {
        Console.WriteLine("ARGS");
        foreach (var item in args)
        {
            Console.WriteLine(item);
        }
        Console.WriteLine("----------------");
    }
}