using System.Reflection;

using GitGet.Model;

using GitPackage.Cli.Model;

using Microsoft.Extensions.Logging;

namespace GitGet.Actions;

internal class About : IAction
{
    private readonly ILogger _appLog;

    public About(ILogger appLog)
    {
        _appLog = appLog;
    }

    public async Task<int> Run(Args args)
    {
        var assem = Assembly.GetExecutingAssembly();

        var ver = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion!;

        using var rd = assem.GetManifestResourceStream(GetType(), "About.md")!;
        var txt2 = new StreamReader(rd).ReadToEnd();

        txt2 = txt2.Replace("https://github.com/Dkowald/kwld.GitPackage", Config.HomeUrl)
            .Replace(".gitget", GitPackageStatusFile.StatusFileName)
            .Replace(".gitpackages", RepositoryCache.DefaultCacheFolderName)
            .Replace("https://github.com/Dkowald/kwld.GitPackage.git", Config.HomeRepositoryUrl);

        Console.Out.Write(txt2);
        return 0;

        //var txt = new string[]
        //{"## dotnet gitget [Action] [options]",
        // " > A tool to get a set of files from a cloned repository",
        // $" > See [Source]({Config.HomeUrl}) for details",
        // $" > Version: {ver}",
        // "-------",
        // "## Action",
        // ". about  - show this info",
        //$". init  - create / update a {GitPackageStatusFile.StatusFileName} package status file",
        //$". info   - show info on cached repositories, and target {GitPackageStatusFile.StatusFileName} if found",
        // ". where  - show local cache clone for specified origin",
        //$". target-path specify folder for {GitPackageStatusFile.StatusFileName} file; defaults to current.",
        // "------",
        // "## Options",
        // ". --origin:[origin]  - source repository url",
        // ". --version:[version]  - source branch/[branch] or tag/[tag]",
        // ". --filter:[filter] - set of ',' seperated globs for target files defaults to all",
        //$". --cache:[cache]  - alternate local cache folder (defaults to HOME/{RepositoryCache.DefaultCacheFolderName}",
        // ". --force:[force]  - force re-get even if already have a commit for [b]ranch, [t]ag or [a]ll",
        // ". --log-level:[LoggingLevel] - [t]race, [d]ebug, [i]nfo, [w]arn, [e]rror",
        // ". --cache:[Cache] - (optional) specify cache for cloned repositories",
        // "--------",
        // $"## {GitPackageStatusFile.StatusFileName} Status File",
        // "> Simple text file with key=value lines",
        // "> when found its values are used as defaults for corresponding options",
        // "Origin=[origin]",
        // "Version=[version]",
        // "Filter=[filter]",
        // "Commit=[commit]",
        // "> commit is set when files are retrieved, if it exists, then it is assumed the files have been collected already",

        // "## E.g",
        // "get the readme and docs files for this project",
        //$" dotnet git-get --origin:{Config.HomeRepositoryUrl} --filter:/*.md;doc/**/*"
        //};

        //foreach (var line in txt)
        //{
        //    Console.WriteLine(line);
        //}

        //return Task.FromResult(0);
    }
}