using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.App_Assets;
using GitPackage.Tests.TestHelpers;

namespace GitPackage.Tests.Flow;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class Basic
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Flow", "Basic", "CorUtil");

    [Ordered, Fact]
    public async Task ResetWorkFolder()
    {
        _root.EnsureExists()
            .EnsureEmpty();

        _root.FileSystem.Project().SetCurrentDirectory();
    }

    private IFileInfo StatusFile => _root.GetFile(GitGetStatus.StatusFileName);

    [Ordered, Fact]
    public async Task CreateTargetStatusFile()
    {
        await using (var wr = StatusFile.Create())
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
            "-f", StatusFile.FullName
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }

    
}