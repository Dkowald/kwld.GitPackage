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
        var files = new FileSystem();
        var log = new FakeLogger();
        var args = Array.Empty<string>();

        var orgHome = Environment.GetEnvironmentVariable("HOME");
        try {
            Environment.SetEnvironmentVariable("HOME", "c:/temp");
            var target = Args.Load(new FileSystem(), log, LogLevel.Debug, args);

            var expectedCache = files.DirectoryInfo
                .New($@"c:\temp\{Args.DefaultCacheFolderName}");

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

        using var _ = new EnvironmentVar("HOME", @"c:\temp\cache");

        var args = Args.Load(files, new FakeLogger(), LogLevel.Trace, []);

        Assert.NotNull(args);

        Assert.Equal(files.Current().FullName, args.TargetPath.FullName);
        Assert.Equal(ActionOptions.Get, args.Action);
        Assert.Equal($@"c:\temp\cache\{Args.DefaultCacheFolderName}", args.Cache.FullName);

        Assert.Equal(files.Current().FullName, args.TargetPath.FullName);
    }
}
