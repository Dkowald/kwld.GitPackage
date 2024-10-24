using System.IO.Abstractions.TestingHelpers;

using GitGet.Actions;
using GitGet.Model;
using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.Model;

public class ArgsTests
{
    [Fact]
    public void ReadLogLevel_BadInput()
    {
        var logLevel = Args.ReadLogLevel([]);
        Assert.Equal(Args.DefaultLogLevel, logLevel);

        Assert.ThrowsAny<Exception>(() => {
            Args.ReadLogLevel(["--log-level:x"]);
        });
    }

    [Fact]
    public void Load_WithDefaultCache()
    {
        var files = new MockFileSystem();
        var log = new FakeLogger();
        var args = Array.Empty<string>();

        var orgHome = Environment.GetEnvironmentVariable("HOME");
        try {
            var home = files.Current().GetFolder("temp");

            Environment.SetEnvironmentVariable("HOME", home.FullName);
            var target = Args.Load(new FileSystem(), log, LogLevel.Debug, args);

            var expectedCache = home.GetFolder(Args.DefaultCacheFolderName);

            Assert.NotNull(target);
            Assert.Equal(expectedCache.FullName, target.Cache.FullName);
        } finally {
            Environment.SetEnvironmentVariable("HOME", orgHome);
        }
    }

    [Fact]
    public void Load_Default()
    {
        var files = new MockFileSystem();

        var home = files.Current().GetFolder(@"\temp\cache");

        using var _ = new EnvironmentVar("HOME", home.FullName);

        var args = Args.Load(files, new FakeLogger(), LogLevel.Trace, []);

        Assert.NotNull(args);

        Assert.Equal(files.Current().FullName, args.TargetPath.FullName);
        Assert.Equal(ActionOptions.Get, args.Action);
        Assert.Equal(home.GetFolder(Args.DefaultCacheFolderName).FullName, args.Cache.FullName);

        Assert.Equal(files.Current().FullName, args.TargetPath.FullName);
    }
}
