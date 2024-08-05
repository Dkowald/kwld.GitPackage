using GitGet;

using GitPackage.Tests.TestHelpers;

namespace GitPackage.Tests.Usage;

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

    //todo: move to its own sample.
    [Ordered, Fact]
    public async Task GetCommonProtos() 
    {
        var targetFolder = Files.AppData.GetFolder("CommonProtos");

        var origin = "https://github.com/protocolbuffers/protobuf.git";
        var version = "branch/main";
        var filter = "/src/google/**/*.proto";

        var args = new[]
        {
            $"{targetFolder.FullName}",
            $"--origin:{origin}",
            $"--version:{version}",
            $"--filter:{filter}"
        };

        await Program.Main(args);

        //await VerifyDirectory(targetFolder.FullName);
    }

    [Ordered, Fact]
    public async Task ReRootProtos()
    {
        var targetFolder = Files.AppData.GetFolder("CommonProtos");

        var origin = "https://github.com/protocolbuffers/protobuf.git";
        var version = "branch/main";
        var filter = "/src/google/**/*.proto";

        var args = new[]
        {
            $"{targetFolder.FullName}",
            $"--origin:{origin}",
            $"--version:{version}",
            $"--filter:{filter}",
            "--get-root:/src",
            "--log-level:t",
            "--force:all"
        };

        await Program.Main(args);
    }

    [Ordered, Fact]
    public async Task FilterTestProtos()
    {
        var targetFolder = Files.AppData.GetFolder("CommonProtos");

        var origin = "https://github.com/protocolbuffers/protobuf.git";
        var version = "branch/main";
        var filter = "/src/google/**/*.proto";

        var ignore = "*unittest*,test_*";

        var args = new[]
        {
            $"{targetFolder.FullName}",
            $"--origin:{origin}",
            $"--version:{version}",
            $"--filter:{filter}",
            $"--ignore:{ignore}",
            "--get-root:/src",
            "--log-level:t",
            "--force:all"
        };

        await Program.Main(args);
    }
}