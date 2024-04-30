using GitPackage.Model;

namespace GitPackage.Tests.Model
{
    public class SourceRepositoryTests
    {
        [Fact]
        public void LocalRepoCahcePath()
        {
            var root = new FileSystem().Project()
                .GetFolder("App_Data", "Cache");

            var target = new SourceRepository(root, "c:/some/path/to/repo");

            var cachePath = target.LocalPath.FullName.ToLower();
            var expectedPath = "local/repo";

            Assert.True(cachePath.EndsWith(expectedPath));
        }
    }
}
