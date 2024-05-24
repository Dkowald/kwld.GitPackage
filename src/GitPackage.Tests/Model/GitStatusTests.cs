using System.IO.Abstractions.TestingHelpers;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;

namespace GitPackage.Tests.Model;

public class GitGetStatusTests
{
    private readonly IDirectoryInfo _root = new MockFileSystem()
        .Current();
        
    [Fact]
    public async Task IsMatch_()
    {
        var localFolder = _root.GetFolder("noMatch")
            .EnsureExists();
        
        var target = new GitGetStatus(localFolder, TestRepository.TestRepositoryUrl, new("branch/dev"));

        Assert.False(target.IsMatch());

        await target.SetMatched();

        Assert.True(target.IsMatch());
    }

    [Fact]
    public async Task IsMatch_CorruptedStatusFile()
    {
        var localFolder = _root.GetFolder("noMatch")
            .EnsureExists();

        var target = new GitGetStatus(localFolder, TestRepository.TestRepositoryUrl, new("branch/dev"));

        Assert.False(target.IsMatch());

        await target.SetMatched();

        var localFile = localFolder.GetFile(GitPackageStatusFile.StatusFileName);

        await localFile.WriteAllTextAsync("corrupted-file-content");
        
        Assert.False(target.IsMatch());

        Assert.False(localFile.Exists());
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