using GitPackage.Cli.GitCommands;
using GitPackage.Cli.Model;

namespace GitPackage.Tests.GitCommands
{
    public class GitGetTests
    {
        private readonly IDirectoryInfo OutRoot = 
            new FileSystem().Project().GetFolder("App_Data", "GitGet");

        [Fact]
        public async Task GetByAnnotatedTag()
        {
            var repo = TestRepository.OpenTestRepository();
            var commit = new GitRef("tag/CheckoutAll");
            var destRoot = OutRoot.GetFolder("Tag0")
                .EnsureEmpty();

            var target = new GitGet(repo);

            target.Run(destRoot, commit, new());

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetByAnnotatedTag))
                .UseDirectory("Snapshot");
        }

        [Fact]
        public async Task GetFiltered()
        {
            var repo = TestRepository.OpenTestRepository();
            var commit = new GitRef("tag/v0");

            var glob = "/Folder1/**/*.md;/Folder2/**/*";

            var destRoot = OutRoot.GetFolder("Tag0Filtered")
                .EnsureEmpty();

            var target = new GitGet(repo);

            target.Run(destRoot, commit, new(glob));

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetFiltered))
                .UseDirectory("Snapshot");
        }

    }
}
