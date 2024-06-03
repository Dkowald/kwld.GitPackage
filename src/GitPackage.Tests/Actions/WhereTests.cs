using System.IO.Abstractions.TestingHelpers;

using GitGet.Actions;
using GitGet.Model;

using GitPackage.Tests.TestHelpers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace GitPackage.Tests.Actions;

public class WhereTests
{
    [Fact]
    public async Task UrlOnly()
    {
        var files = new MockFileSystem();

        var prjDir = files.DirectoryInfo.New("c://prj");
        var cache = files.DirectoryInfo.New("c://cache");

        var args = new Args(LogLevel.Trace, ActionOptions.Where,
            prjDir, cache)
        {
            Origin = new("http://someurl/repo.git")
        };
        
        var con = new FakeConsole();

        var target = new Where(new FakeLogger(), con);

        var expectedPath = "c:\\cache\\someurl\\repo.git";

        var exitCode = await target.Run(args);

        Assert.Equal(0, exitCode);

        Assert.Equal(expectedPath, con.StdOut);
    }
}