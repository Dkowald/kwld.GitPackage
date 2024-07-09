using Microsoft.Extensions.Logging.Testing;
using GitGet.Model;
using Microsoft.Extensions.Logging;

using GitGet.Actions;
using GitPackage.Tests.TestHelpers;
using GitGet.Utility;

namespace GitPackage.Tests.Actions;

public class GetTests
{
    [Fact]
    public async Task ReportNetworkActivity()
    {
        //pre-cache test repo
        TestRepository.OpenTestRepository().Dispose();

        //new RepositoryCache(new FakeLogger(), Files.TestPackageCacheRoot)
        //    .Purge(TestRepository.BareRepoPath.AsUri());
        //    //.CloneIfMissing(TestRepository.BareRepoPath.AsUri());

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

    [Fact(Skip = "todo")]
    public void RepositoryRequiresCredentials()
    {
        //should report user command to clone.
    }

    [Fact(Skip = "todo")]
    public void NoFetchIfHaveRef()
    {
        //if status file has a commit.
        //assume all done; do no work.

        //Need target to report when it does network activity.
    }
}