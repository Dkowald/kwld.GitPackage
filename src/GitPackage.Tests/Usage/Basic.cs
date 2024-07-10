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
    private IFileInfo StatusFile => _root.GetFile(GitGet.Model.StatusFile.FileName);

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

        var args = new[]
        {
            "init",
            $"--origin:{Origin}",
            "--version:tag/v1.3.1",
            "--filter:/readme.md"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
        Assert.True(StatusFile.Exists());
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
    public async Task GetUpdateWithCli()
    {
        using var _ = new PushD(_root);

        var args = new[]
        {
            "--version:tag/v1.3.2",
            "--log-level:d"
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }
}