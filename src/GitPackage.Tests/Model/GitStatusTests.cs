using System.IO.Abstractions.TestingHelpers;
using GitPackage.Cli.Model;

namespace GitPackage.Tests.Model;

public class GitGetStatusTests
{
    private readonly IDirectoryInfo _root = new MockFileSystem()
        .Current();
        
    [Fact]
    public void IsMatch_()
    {
        var noMatch = _root.GetFolder("noMatch");
        var repo = TestRepository.OpenTestRepository();
        
        var target = new GitGetStatus(noMatch, TestRepository.TestRepositoryUrl, new("branch/dev"));

        Assert.False(target.IsMatch());

        target.SetMatched();

        Assert.True(target.IsMatch());
    }

    [Fact]
    public void IsMatch_ChangePackageInfo()
    {
        var noMatch = _root.GetFolder("noMatch");
        var repo = TestRepository.OpenTestRepository();

        var actual = new GitGetStatus(noMatch, TestRepository.TestRepositoryUrl, new("branch/dev"));
        actual.SetMatched();

        var target = new GitGetStatus(noMatch, TestRepository.TestRepositoryUrl, new("branch/dev2"));
        Assert.False(target.IsMatch());
    }
}