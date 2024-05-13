using GitPackage.Cli.GitCommands;
using GitPackage.Cli.Model;
using GitPackage.Cli.Utility;

using LibGit2Sharp;

using Microsoft.Extensions.Configuration;

namespace GitPackage.Cli;

internal class Program
{
    public static readonly string DefaultCacheFolderName = ".gitpackages";

    internal static async Task<int> Main(string[] args)
    {
        var files = new FileSystem();

        var argInfo = new Args(args);

        if(argInfo.ShowVersion) {
            Version();
            return 0;
        }

        GitPackageItem package = null;
        if (argInfo.DataFile != null)
        {
            var statusFile = files.Current().GetFile(argInfo.DataFile);
            package = GitPackageItem.Load(statusFile);
        }

        if (package is null)
            throw new Exception("No package defined");

        //Init clone.
        var cacheRoot = argInfo.Cache is null ? DefaultCache(files) : files.DirectoryInfo.New(argInfo.Cache);
        var cache = new RepositoryCache(cacheRoot, package.Include);

        if (!cache.CachePath.Exists)
        {
            cache.CachePath.EnsureExists();
            Repository.Clone(cache.Origin.ToString(), cache.CachePath.FullName,
                new CloneOptions { IsBare = true });
        }

        var repo = new Repository(cache.CachePath.FullName);

        //Check for ref.
        var targetRef = repo.Refs[package.Version.ToString()];

        if (targetRef is null)
        {
            var refSpecs = repo.Network.Remotes["origin"].FetchRefSpecs.Select(x => x.Specification);
            Commands.Fetch(repo, "origin", refSpecs, new(){TagFetchMode = TagFetchMode.All}, "");
        }
        targetRef = repo.Refs[package.Version.ToString()];

        if (targetRef is null)
            throw new Exception($"Cannot find {package.Version}");

        //Check if already have the commit in the git get
        var resolveRef = targetRef.ResolveToDirectReference();
        var commit = resolveRef.Target.Peel<Commit>();

        if (package.Commit == commit.Sha)
            return 0;

        new GitGet(repo)
            .Run(
                files.Current().GetFolder(package.Path),
                package.Version, 
                new(package.Filter));

        package.Commit = commit.Sha;

        package.Write();

        return 1;

        EchoArgs(args);

        var config = BuildConfig(args);

        var (error, appArgs) = AppArgs.TryLoad(config);
        if (appArgs is null)
        {
            var logError = error.IsNullOrEmpty() ? "Problem reading arguments" : error;

            await Console.Error.WriteLineAsync(logError);
            return 1;
        }

        Console.WriteLine($"Using cache folder: {appArgs.RepositoryCache}");
        Console.WriteLine($"Processing {appArgs.Packages.Length} items");

        return 0;
    }

    public static IConfigurationRoot BuildConfig(params string[] args)
    {
        var fileSystem = new FileSystem();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new(nameof(AppArgs.RepositoryCache), DefaultCache(fileSystem).FullName)])
            .AddEnvironmentVariables()
            .AddCommandLine(args, new Dictionary<string, string>
            {
                {"--cache", "RepositoryCache"},
                {"-c", "RepositoryCache"},
                {"-i", "Packages"},
                {"--input", "Packages"}
            })
            .Build();

        return config;
    }

    public static async Task<int> Run(AppArgs config, GitPackageItem item)
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

    static void Version()
    {
        Console.WriteLine("Version 0.0.1");
    }

    static void Help()
    {
        Console.WriteLine("GitPackage CLI");
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

    static IDirectoryInfo DefaultCache(IFileSystem fileSystem)
        => (fileSystem.TryGetHome()??fileSystem.Current()).GetFolder(DefaultCacheFolderName);
}