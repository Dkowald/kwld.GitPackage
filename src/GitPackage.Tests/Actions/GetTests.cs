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
        var logs = new List<string>();
        var log = new FakeLogger(logs.Add);

        var outDir = Files.AppData.GetFolder(nameof(GetTests), "Clone");
        var cacheDir = Files.AppData.GetFolder(nameof(GetTests), "Cache");

        cacheDir.ClearReadonly().EnsureEmpty();
        outDir.ClearReadonly()
            .EnsureEmptyWithoutDelete();

        var target = new Get(log);

        var args = new Args(LogLevel.Trace, ActionOptions.Get, outDir, cacheDir);

        await target.Run(args);

        Assert.True(logs.Any(x => x.Contains("Fetch")));
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