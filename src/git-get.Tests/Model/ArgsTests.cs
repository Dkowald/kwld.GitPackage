using GitGet.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.Model;

public class ArgsTests
{
    [Fact]
    public void Load_WithDefaultCache() 
    {
        var files = new FileSystem();
        var log = new FakeLogger();
        var args = Array.Empty<string>();
                
        var orgHome = Environment.GetEnvironmentVariable("HOME");
        try
        {
            Environment.SetEnvironmentVariable("HOME", "c:/temp");
            var target = Args.Load(new FileSystem(), log, LogLevel.Debug, args);
            
            var expectedCache = files.DirectoryInfo
                .New($"c:\\temp\\{Args.DefaultCacheFolderName}");

            Assert.NotNull(target);
            Assert.Equal(expectedCache.FullName, target.Cache.FullName);
        }
        finally {
            Environment.SetEnvironmentVariable("HOME", orgHome);
        }
    }
}
