using System.IO.Abstractions.TestingHelpers;

using GitGet.GitCommands;
using GitGet.Model;
using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.GitCommands;

public class GetTests
{
    private readonly IDirectoryInfo _outRoot = Files.AppData.GetFolder(nameof(GetTests));
    
    [Fact]
    public async Task ReportResult()
    {
        var files = new MockFileSystem();

        using var _ = TestRepository.OpenTestRepository();

        using var repo = new RepositoryCache(new FakeLogger(), Files.TestPackageCacheRoot)
            .CloneIfMissing(TestRepository.BareRepoPath.AsUri(), null);

        var target = new Get(repo);

        var result = await target.Run(files.Current(), new("branch/ManyFiles"),
            new("f*.txt"), new("*.x*"), new("/manyFiles"));

        Assert.Equal(8, result.TotalItems);
        Assert.Equal(6, result.IncludedItemsCount);
        Assert.Equal(2, result.IgnoredItemsCount);

        var allFiles = files.Current().EnumerateFiles("*", SearchOption.AllDirectories);
        Assert.Equal(result.IncludedItemsCount - result.IgnoredItemsCount, allFiles.Count());
    }

    [Fact]
    public async Task GetSubPath()
    {
        using var repo = TestRepository.OpenTestRepository();

        var target = new Get(repo);

        var destRoot = _outRoot.GetFolder("GetSubPath").EnsureEmpty();

        var version = new GitRef("tag/v1");

        await target.Run(destRoot, version, new("/Folder2/Folder1/*.txt"), null, new("/Folder2"));

        await VerifyDirectory(destRoot.FullName);
    }

    [Fact]
    public async Task GetByAnnotatedTag()
    {
        using var repo = TestRepository.OpenTestRepository();
        var version = new GitRef("tag/CheckoutAll");
        var destRoot = _outRoot.GetFolder("Tag0")
            .EnsureEmpty();

        var target = new Get(repo);

        await target.Run(destRoot, version);

        await VerifyDirectory(destRoot.FullName);
    }

    [Fact]
    public async Task GetFiltered()
    {
        using var repo = TestRepository.OpenTestRepository();
        var commit = new GitRef("tag/v0");

        var glob = "*.md,Folder2/**/*";

        var destRoot = _outRoot.GetFolder(nameof(GetFiltered))
            .EnsureEmpty();

        var target = new Get(repo);

        await target.Run(destRoot, commit, new(glob));

        await VerifyDirectory(destRoot.FullName);
    }

    [Fact]
    public async Task GetNoAnchorFiltered()
    {
        //need cloned repo so have origin.
        using var repo = new RepositoryCache(new FakeLogger(), Files.TestPackageCacheRoot)
            .CloneIfMissing(TestRepository.BareRepoPath.AsUri(), null);

        var commit = new GitRef("branch/IncludeNestedSameNameFolder");

        var glob = "**/folder1/*.txt";

        var destRoot = _outRoot.GetFolder(nameof(GetNoAnchorFiltered))
            .EnsureEmpty();

        var target = new Get(repo);

        await target.Run(destRoot, commit, new(glob));

        await VerifyDirectory(destRoot.FullName);
    }

    [Fact]
    public async Task GetFilteredWithRoot()
    {
        using var repo = new RepositoryCache(new FakeLogger(), Files.TestPackageCacheRoot)
            .CloneIfMissing(TestRepository.BareRepoPath.AsUri(), null);

        var target = new Get(repo);

        var destRoot = _outRoot.GetFolder(nameof(GetNoAnchorFiltered))
            .EnsureEmpty();

        await target.Run(destRoot, new("branch/ManyFiles"), new("dir1/*.x.txt"), null, new("/manyfiles/dir1/"));

        await VerifyDirectory(destRoot.FullName);
    }
}