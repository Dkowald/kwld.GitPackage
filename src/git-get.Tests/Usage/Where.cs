using GitGet;

using GitPackage.Tests.TestHelpers;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class Where
{
    public const string RepoUrl = "https://github.com/rsafier/DotNetGlob.git";

    [Fact]
    public async Task CheckInfoAboutCachedRepo()
    {
        var cache = Files.TestPackageCacheRoot;

        var args = new[]
        {
            "where",
            $"--cache:{cache.FullName}",
            $"--origin:{RepoUrl}",
            "--log-level:t"
        };

        var expected = cache.GetFolder("github.com/rsafier/DotNetGlob.git");

        using var con = new CaptureConsole();
        await Program.Main(args);
        con.Flush();

        var repoPath = con.Flush().StdOut;

        Assert.Equal(expected.FullName, repoPath);
    }
}
