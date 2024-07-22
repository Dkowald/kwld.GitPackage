using System.IO.Abstractions.TestingHelpers;

using GitGet.Model;

using Microsoft.Extensions.Logging.Testing;

namespace GitPackage.Tests.Model;

public class StatusFileTests
{
    [Fact]
    public async Task TryLoadWithOverrides_()
    {
        var files = new MockFileSystem();

        var workDir = files.Current();

        var current = await new StatusFile(workDir, new("http://someurl"), new("tag/1"), new("*.md"))
        {
            Commit = "commit"
        }.Write(new FakeLogger());

        var args = new Args(
            Microsoft.Extensions.Logging.LogLevel.Trace,
            GitGet.Actions.ActionOptions.About,
            files.Current(), files.Current())
        {
            Origin = new("http://updatedurl")
        };

        var result = await StatusFile.TryLoadWithOverrides(new FakeLogger(), args);

        Assert.NotNull(result);
        
        Assert.Equal(current.Filter, result.Filter);
        Assert.Equal(args.Origin, result.Origin);
        Assert.Null(result.Commit);
    }

    [Fact(Skip = "Todo")]
    public void LoadNoStatusFile()
    {

    }
}
