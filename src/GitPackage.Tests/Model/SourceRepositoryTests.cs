using GitPackage.Cli.Model;

namespace GitPackage.Tests.Model
{
    public class RepositoryCacheTests
    {
        [Fact]
        public void LocalRepoCachePath()
        {
            var root = new FileSystem().Project()
                .GetFolder("App_Data", "Cache");

            var target = new RepositoryCache(root, "c:/some/path/to/repo");

            var cachePath = target.CachePath.FullName.Replace('\\','/').ToLower();
            var expectedPath = "local/repo";

            Assert.True(cachePath.EndsWith(expectedPath));
        }
    }
}
