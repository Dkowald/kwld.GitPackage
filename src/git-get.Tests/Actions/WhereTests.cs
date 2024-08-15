using System.IO.Abstractions.TestingHelpers;

using GitGet.Actions;
using GitGet.Model;
using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitGet.Tests.Actions;

public class WhereTests
{
    [Fact]
    public async Task UrlOnly()
    {
        using var host = new TestHost(x => { x.AddSingleton<Where>(); });

        var files = new MockFileSystem();

        var prjDir = files.DirectoryInfo.New("c://prj");
        var cache = files.DirectoryInfo.New("c://cache");

        var args = new Args(LogLevel.Trace, ActionOptions.Where,
            prjDir, cache) {
            Origin = new("http://someurl/repo.git")
        };

        var target = host.Get<Where>();

        var expectedPath = "c:\\cache\\someurl\\repo.git";

        var exitCode = await target.Run(args);

        //should be no log entries, so url returned can be used directly
        Assert.Empty(host.LogEntries);

        Assert.Equal(0, exitCode);

        Assert.Equal(expectedPath, host.StdOut);
    }
}