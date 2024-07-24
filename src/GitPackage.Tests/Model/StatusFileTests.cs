using System.IO.Abstractions.TestingHelpers;

using GitGet.Model;

using Microsoft.Extensions.Logging.Testing;

namespace GitPackage.Tests.Model;

public class StatusFileTests
{
    [Fact]
    public async Task LoadWithArgumentOverides_()
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

        var result = await StatusFile.LoadWithArgumentOverides(new FakeLogger(), args);

        Assert.NotNull(result);
        
        Assert.Equal(current.Filter, result.Filter);
        Assert.Equal(args.Origin, result.Origin);
        Assert.Null(result.Commit);
    }

    [Fact]
    public async Task Write_Read()
    {
        var log = new FakeLogger();

        var files = new MockFileSystem();

        var dir = files.Current();

        var original = new StatusFile(dir, new("http://somewhere"), new("tag/1"), new())
        {
            Commit = "zzzzzzzz1"
        };

        await original.Write(log);

        var result = await StatusFile.TryLoad(log, files.Current());

        Assert.NotNull(result);

        Assert.Equal(original, result);
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
