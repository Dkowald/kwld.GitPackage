using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.Model
{
    public class RepositoryCacheTests
    {
        [Fact]
        public void LocalRepoCachePath()
        {
            using var host = new TestHost();
            var appLogger = host.Get<ILoggerFactory>().CreateLogger("");

            var root = new FileSystem().Project()
                .GetFolder("App_Data", "Cache");

            var target = RepositoryCache.New(appLogger, root, "c:/some/path/to/repo");

            Assert.NotNull(target);

            var cachePath = target.CachePath.FullName.Replace('\\','/').ToLower();
            var expectedPath = "local/repo";

            Assert.True(cachePath.EndsWith(expectedPath));
        }
    }
}
