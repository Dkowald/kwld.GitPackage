using GitGet;
using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class Basic
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Flow", "Basic", "CorUtil");
    private IFileInfo StatusFile => _root.GetFile(GitPackageStatusFile.StatusFileName);

    [Ordered, Fact]
    public void ResetWorkFolder()
    {
        _root.EnsureExists()
            .EnsureEmpty();

        _root.FileSystem.Project().SetCurrentDirectory();
    }


    [Ordered, Fact]
    public void CreateTargetStatusFile()
    {
        var logger = Substitute.For<ILogger>();
        
        new GitPackageStatusFile(logger, StatusFile)
        {
            Origin = "https://github.com/Dkowald/kwld.CoreUtil.git",
            Version = new("tag/v1.3.2"),
            Filter = new("*.md;docs/*.md")
        }
        .Write();
    }

    [Ordered, Fact]
    public async Task RunGitGet()
    {
        var args = new[]
        {
            StatusFile.Directory!.FullName,
            $"--cache:{Files.TestPackageCacheRoot.FullName}",
            "--log-level:t"
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }

    
}