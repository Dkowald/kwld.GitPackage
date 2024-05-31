using GitGet;
using GitGet.Model;
using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class Basic
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Usage", "Basic");
    private IFileInfo StatusFile => _root.GetFile(nameof(GetUsingStatusFile), GitPackageStatusFile.StatusFileName);

    [Ordered, Fact]
    public void ResetWorkFolder()
    {
        _root.EnsureExists()
            .EnsureEmpty();
    }
    
    [Ordered, Fact]
    public void CreateTargetStatusFile()
    {
        var logger = Substitute.For<ILogger>();
        
        new GitPackageStatusFile(StatusFile, 
                new("https://github.com/Dkowald/kwld.CoreUtil.git"),
                new("tag/v1.3.2"),
                new("/*.md;docs/*.md"))
        .Write(logger);
    }
    
    [Ordered, Fact]
    public async Task GetUsingStatusFile()
    {
        var args = new[]
        {
            $"--cache:{Files.TestPackageCacheRoot.FullName}",
            "--log-level:t",
            "--force:all"
        };

        int result;
        
        using (new PushD(StatusFile.Directory!))
            result = await Program.Main(args);

        Assert.Equal(0, result);
        
        await VerifyDirectory(StatusFile.Directory!.FullName);
    }

    [Ordered, Fact]
    public async Task GetReadme()
    {
        var cwd = _root.GetFolder(nameof(GetReadme));

        using var _ = new PushD(cwd.EnsureExists());

        var args = new[]
        {
            "--origin:https://github.com/libgit2/libgit2sharp.git",
            "--version:branch/maint/v0.22",
            "--filter:/readme.md",
            "--log-level:d"
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(cwd.FullName);
    }
}