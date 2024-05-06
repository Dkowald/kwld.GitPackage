using GitPackage.Cli.Model;
using GitPackage.Cli.Tasks;
using GitPackage.Tests.TestHelpers;

namespace GitPackage.Tests.Tasks
{
    public class CloneIfMissingTests
    {
        [Fact]
        public void Run_NewRepo()
        {
            var srcRepo = new RepositoryCache(
                Files.TestPackageCacheRoot,
                "https://github.com/Dkowald/kwld.CoreUtil.git");

            srcRepo.CachePath
                .ClearReadonly()
                .EnsureDelete();
            
            var target = new CloneIfMissing(srcRepo, null);

            target.Run();

            var _ = target.Repository;
        }
    }
}
