using Microsoft.Extensions.Logging.Testing;

namespace GitPackage.Tests.Actions;

public class GetTests
{

    [Fact]
    public void ReportNetworkActivity()
    {
        var logs = new List<string>();
        var log = new FakeLogger(logs.Add);

        var target = new GitGet.Actions.Get(log);

        target.Run();
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