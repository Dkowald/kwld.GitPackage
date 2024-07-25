using System.IO.Abstractions.TestingHelpers;

using GitGet.Actions;
using GitGet.Model;
using GitGet.Utility;

using GitPackage.Tests.TestHelpers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GitPackage.Tests.Actions;

public class GetTests
{
    readonly IDirectoryInfo Root = Files.AppData.GetFolder(nameof(GetTests));

    [Fact]
    public async Task ReportNetworkActivity()
    {
        //pre-cache test repo
        TestRepository.OpenTestRepository().Dispose();

        //get master.
        using var host = new TestHost();

        var outDir = Files.AppData.GetFolder(nameof(GetTests), "OutDir");
        
        outDir.ClearReadonly()
            .EnsureEmptyWithoutDelete();

        var target = new Get(host.Get<ILogger>());

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
            x.AddSingleton<Get>();
        });

        var dir = new MockFileSystem().Current();

        var data = new StatusFile(dir, new("https://goes-no-where"), new("branch/main"), new())
        { Commit = "already-have-commit"}.Write(host.Get<ILogger>());

        var target = host.Get<Get>();

        var args = new Args(LogLevel.Debug, ActionOptions.Get, dir, dir);

        var exitCode = await target.Run(args);

        var noWork = host.LogEntries.Any(x => x.Contains("no work"));

        Assert.True(noWork);
    }

    [Fact(Skip = "todo")]
    public void RepositoryHasProtectedFile()
    {
        //add StatusFile.FileName to the test repo branch.
        //verify the true status file created.
        //verify warning generated.
    }

    [Fact(Skip ="todo: add ability to re-root, so use with mono-repositories is easier.")]
    public void ReRoot()
    {

    }

    [Fact(Skip = "should create dir, and report as info.")]
    public void TargetPathNotExist()
    {

    }
}