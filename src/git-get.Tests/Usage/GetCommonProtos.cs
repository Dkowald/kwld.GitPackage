using GitGet.Tests.TestHelpers;

namespace GitGet.Tests.Usage;

/// <summary>
/// Get the common proto but protos from google
/// </summary>
[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class GetCommonProtos
{
    private static readonly IDirectoryInfo Working = Files.AppData
        .GetFolder("Usage", "CommonProtos");

    [Ordered, Fact]
    public void Init()
    {
        Working.EnsureEmpty();
    }

    [Ordered, Fact]
    public async Task AllProtos()
    {
        var origin = "https://github.com/protocolbuffers/protobuf.git";
        var version = "v27.3";
        var filter = "/src/google/**/*.proto";

        var args = new[] {
            $"{Working.FullName}",
            $"--origin:{origin}",
            $"--version:{version}",
            $"--filter:{filter}"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
    }

    [Ordered, Fact]
    public async Task ReRoot()
    {
        var args = new[]
        {
            $"{Working.FullName}",
            "--get-root:/src"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
    }

    [Ordered, Fact]
    public async Task IgnoreTestProtos()
    {
        var ignore = "*unittest*,test_*,_test*";

        var args = new[]
        {
            $"{Working.FullName}",
            $"--ignore:{ignore}"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
    }

    [Ordered, Fact]
    public async Task GetLatestOnMain()
    {
        var version = "branch/main";

        var args = new[]
        {
            $"{Working.FullName}",
            $"--version:{version}"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
    }
}