using GitGet;
using GitGet.Model;
using GitGet.Utility;

using GitPackage.Tests.TestHelpers;

using Microsoft.Extensions.Logging;

using NSubstitute;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class Basic
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Usage", nameof(Basic));
    private IDirectoryInfo StatusFolder => _root.GetFolder(StatusFile.StatusFolder);

    private readonly string Origin = "https://github.com/Dkowald/kwld.CoreUtil.git";

    [Ordered, Fact]
    public void MakeNewWorkingFolder()
    {
        _root.EnsureExists()
            .EnsureEmptyWithoutDelete();
    }

    [Ordered, Fact]
    public async Task InitTargetStatusFile()
    {
        using var _ = new PushD(_root);

        var logger = Substitute.For<ILogger>();

        var args = new[]
        {
            "init",
            $"--origin:{Origin}",
            "--version:tag/v1.3.2",
            "--filter:/*.md,docs/*.md"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
        Assert.True(StatusFolder.Exists());
    }

    [Ordered, Fact]
    public async Task GetUsingStatusFile()
    {
        var args = new[]
        {
            "--log-level:t",
            "--force:all"
        };

        int result;

        using (new PushD(_root))
            result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }

    [Ordered, Fact]
    public async Task GetReadme()
    {
        using var _ = new PushD(_root);

        var args = new[]
        {
            "--origin:https://github.com/libgit2/libgit2sharp.git",
            "--version:branch/maint/v0.22",
            "--filter:/readme.md",
            "--log-level:d"
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }
}