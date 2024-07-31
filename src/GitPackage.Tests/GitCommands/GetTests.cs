using GitGet.Model;
using GitPackage.Tests.TestHelpers;

using Microsoft.Extensions.Logging.Testing;

namespace GitPackage.Tests.GitCommands
{
    public class GetTests
    {
        private readonly IDirectoryInfo _outRoot = 
            new FileSystem().Project().GetFolder("App_Data", "GitGet");

        [Fact]
        public async Task Run_GetSubPath()
        {
            using var repo = TestRepository.OpenTestRepository();

            var target = new GitGet.GitCommands.Get(repo);

            var destRoot = _outRoot.GetFolder("GetSubPath").EnsureEmpty();

            var version = new GitRef("tag/CheckoutAll");

            await target.Run(destRoot, version, new(), "/Folder2");

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(Run_GetSubPath))
                .UseDirectory("Snapshot");
        }

        [Fact]
        public async Task GetByAnnotatedTag()
        {
            using var repo = TestRepository.OpenTestRepository();
            var version = new GitRef("tag/CheckoutAll");
            var destRoot = _outRoot.GetFolder("Tag0")
                .EnsureEmpty();

            var target = new GitGet.GitCommands.Get(repo);

            await target.Run(destRoot, version, new());

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetByAnnotatedTag))
                .UseDirectory("Snapshot");
        }

        [Fact]
        public async Task GetFiltered()
        {
            using var repo = TestRepository.OpenTestRepository();
            var commit = new GitRef("tag/v0");

            var glob = "Folder1/**/*.md,Folder2/**/*";

            var destRoot = _outRoot.GetFolder("Tag0Filtered")
                .EnsureEmpty();

            var target = new GitGet.GitCommands.Get(repo);

            await target.Run(destRoot, commit, new(glob));

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetFiltered))
                .UseDirectory("Snapshot");
        }

        [Fact]
        public async Task GetNoAnchorFiltered() 
        {
            //need cloned repo so have origin.
            using var repo = new RepositoryCache(new FakeLogger(), Files.TestPackageCacheRoot)
                .CloneIfMissing(TestRepository.BareRepoPath.AsUri(), null);

            var commit = new GitRef("branch/IncludeNestedSameNameFolder");

            var glob = "**/folder1/*.txt";

            var destRoot = _outRoot.GetFolder(nameof(GetNoAnchorFiltered))
                .EnsureEmpty();

            var target = new GitGet.GitCommands.Get(repo);

            await target.Run(destRoot, commit, new(glob));

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetNoAnchorFiltered))
                .UseDirectory("Snapshot");
        }
    }
}
