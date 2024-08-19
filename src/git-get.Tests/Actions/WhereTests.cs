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
        var root = files.Current();

        var prjDir = root.GetFolder("prj");
        var cache = root.GetFolder("cache");
        
        var args = new Args(LogLevel.Trace, ActionOptions.Where,
            prjDir, cache) {
            Origin = new("http://someurl/repo.git")
        };
        
        var target = host.Get<Where>();

        var expectedPath = cache.GetFolder("someurl", "repo.git").FullName;

        var exitCode = await target.Run(args);

        //should be no log entries, so url returned can be used directly
        Assert.Empty(host.LogEntries);

        Assert.Equal(0, exitCode);

        Assert.Equal(expectedPath, host.StdOut);
    }
}