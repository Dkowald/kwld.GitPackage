using GitPackage.Model;

using LibGit2Sharp;

namespace GitPackage.Tests.Tasks
{
    public class CloneTests
    {
        [Fact]
        public void CloneNewRepo()
        {
            var cacheRoot = new FileSystem().Project()
                .GetFolder("App_Data").GetFolder(".gitpackages");

            var srcRepo = new SourceRepository(cacheRoot, "https://github.com/Dkowald/kwld.CoreUtil.git");

            if (!srcRepo.LocalPath.Exists)
            {
                Repository.Clone(srcRepo.Origin.ToString(),
                    srcRepo.LocalPath.FullName, new()
                    {
                        IsBare = true,
                        FetchOptions = { }
                    });
            }

            var repo = new Repository(srcRepo.LocalPath.FullName);
        }
    }
}
