using System.IO.Abstractions.TestingHelpers;

using GitGet.Actions;
using GitGet.Model;
using GitGet.Utility;

using GitPackage.Tests.TestHelpers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitPackage.Tests.Actions;

public class GetActionTests
{
    [Fact]
    public async Task ReportNetworkActivity()
    {
        //pre-cache test repo
        TestRepository.OpenTestRepository().Dispose();

        //get master.
        using var host = new TestHost();

        var outDir = Files.AppData.GetFolder(nameof(GetActionTests), "OutDir");
        
        outDir.ClearReadonly()
            .EnsureEmptyWithoutDelete();

        var target = new GetAction(host.Get<ILogger>());

        var args = new Args(LogLevel.Trace, ActionOptions.Get, outDir, Files.TestPackageCacheRoot)
        {
            Origin = TestRepository.BareRepoPath.AsUri(),
            Version = new("branch/master")
        };

        var exitCode = await target.Run(args);
        
        Assert.Equal(0, exitCode);

        Assert.Contains(host.LogEntries, x => x.Contains("Fetch"));
    }

    [Fact]
    public async Task NoFetchIfHaveRef()
    {
        using var host = new TestHost(x =>
        {
            x.AddSingleton<GetAction>();
        });

        var dir = new MockFileSystem().Current();

        await new StatusFile(dir, new("https://goes-no-where"), new("branch/main"), GlobFilter.MatchAll)
            { Commit = "already-have-commit"}.Write(host.Get<ILogger>());

        var target = host.Get<GetAction>();

        var args = new Args(LogLevel.Debug, ActionOptions.Get, dir, dir);

        await target.Run(args);

        var noWork = host.LogEntries.Any(x => x.Contains("no work"));

        Assert.True(noWork);
    }

    [Fact]
    public async Task Run_RepositoryHasProtectedFile()
    {
        using var repo = TestRepository.OpenTestRepository();

        using var host = new TestHost(x => x.AddSingleton<GetAction>());

        var target = host.Get<GetAction>();

        var args = new Args(LogLevel.Warning, ActionOptions.Get,
            Files.AppData.GetFolder(nameof(GetActionTests)),
            Files.TestPackageCacheRoot)
        {
            Cache = Files.TestPackageCacheRoot,
            Origin = TestRepository.BareRepoPath.AsUri(),
            Version = new("branch/BranchHasStatusFile")
        };

        args.TargetPath.EnsureEmpty();

        await target.Run(args);

        var warnOverwrite = host.LogEntries.Count(x => x.Contains(
            $"Extracted file {StatusFile.FileName} is being overwritten"));

        Assert.True(warnOverwrite > 0);

        var statusFile = args.TargetPath.GetFile(StatusFile.FileName);
        await VerifyFile(statusFile.FullName);
    }
}