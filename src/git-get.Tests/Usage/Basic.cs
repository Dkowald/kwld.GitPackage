using GitGet.Tests.TestHelpers;

namespace GitGet.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class Basic
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Usage", nameof(Basic));
    private IFileInfo StatusFile => _root.GetFile(GitGet.Model.StatusFile.FileName);

    private readonly string _origin = "https://github.com/Dkowald/kwld.CoreUtil.git";

    [Ordered, Fact]
    public void Init()
    {
        _root.EnsureEmpty();
    }

    [Ordered, Fact]
    public async Task InitTargetStatusFile()
    {
        var args = new[]
        {
            "init",
            $"--target-path:{_root.FullName}",
            $"--origin:{_origin}",
            "--version:tag/v1.3.1",
            "--filter:/readme.md, other.txt"
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
            $"{_root.FullName}",
            "--log-level:t",
            "--force:all"
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }

    [Ordered, Fact]
    public async Task GetUpdateWithCli()
    {
        var args = new[]
        {
            $"{_root.FullName}",
            "--version:tag/v1.3.2",
            "--log-level:d"
        };

        var result = await Program.Main(args);

        Assert.Equal(0, result);

        await VerifyDirectory(_root.FullName);
    }
}