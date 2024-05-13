using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.App_Assets;
using GitPackage.Tests.TestHelpers;

namespace GitPackage.Tests
{
    [TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
    public class ProgramTests
    {
        private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Program", "CorUtil");

        [Ordered, Fact]
        public async Task ResetWorkFolder()
        {
            _root.EnsureEmpty();

            _root.FileSystem.Project().SetCurrentDirectory();
        }
        
        private IFileInfo _statusFile => _root.GetFile(GitGetStatus.StatusFileName);

        [Ordered, Fact]
        public async Task CreateTargetStatusFile()
        {
            await using (var wr = _statusFile!.Create())
            {
                await Assets.CoreUtilGitPackage().CopyToAsync(wr);
            }
        }

        [Ordered, Fact]
        public async Task RunGitGet()
        {
            var args = new[]
            {
                "-c", Files.TestPackageCacheRoot.FullName,
                "-f", _statusFile.FullName
            };

            var result = await Program.Main(args);

            Assert.Equal(0, result);

            await VerifyDirectory(_root.FullName);

        }

    }
}
