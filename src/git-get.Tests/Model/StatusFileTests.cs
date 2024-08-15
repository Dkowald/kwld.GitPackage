using System.IO.Abstractions.TestingHelpers;

using GitGet.Actions;
using GitGet.Model;
using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.Model;

public class StatusFileTests
{
    [Fact]
    public async Task TryLoadWithOverrides_()
    {
        var files = new MockFileSystem();

        var workDir = files.Current();

        var current = await new StatusFile(workDir, new("http://someurl"), new("tag/1"), new("*.md")) {
            Commit = "commit",
            GetRoot = new("/some/where")
        }.Write(new FakeLogger());

        var args = new Args(
            LogLevel.Trace,
            ActionOptions.About,
            files.Current(), 
            files.Current()) {
            Origin = new("http://updatedurl"),
        };

        var result = await StatusFile.TryLoadWithOverrides(new FakeLogger(), args);

        var package = result.File;

        Assert.NotNull(package);

        Assert.Equal(current.Filter, package.Filter);
        Assert.Equal(args.Origin, package.Origin);
        Assert.Null(package.Commit);
    }

    [Fact]
    public async Task Write_Read()
    {
        var log = new FakeLogger();

        var dir = Files.AppData.GetFolder(nameof(StatusFileTests), nameof(Write_Read))
            .EnsureEmpty();

        var original = new StatusFile(dir,
            new("http://somewhere"),
            new("tag/1"), GlobFilter.MatchAll) {
            Commit = "zzzzzzzz1",
            GetRoot = new("/aplace")
        };

        await original.Write(log);

        var result = await StatusFile.TryLoad(log, dir);

        Assert.NotNull(result);

        Assert.True(original.Same(result));

        await VerifyFile(dir.GetFile(StatusFile.FileName).FullName);
    }

    [Fact]
    public async Task TryLoad_NoFile()
    {
        var log = new FakeLogger();

        var files = new MockFileSystem();

        var result = await StatusFile.TryLoad(log, files.Current());

        Assert.Null(result);
    }
}
