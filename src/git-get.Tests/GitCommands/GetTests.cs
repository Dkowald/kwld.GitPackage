using GitGet.Model;
using GitGet.Tests.TestHelpers;
using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.GitCommands;

public class GetTests
{
    private readonly IDirectoryInfo _outRoot = Files.AppData.GetFolder(nameof(GetTests));

    [Fact(Skip = "true")]
    public void ReportResult()
    {
        //Report totals: found; extracted and ignored
    }

    [Fact]
    public async Task GetSubPath()
    {
        using var repo = TestRepository.OpenTestRepository();

        var target = new GitGet.GitCommands.Get(repo);

        var destRoot = _outRoot.GetFolder("GetSubPath").EnsureEmpty();

        var version = new GitRef("tag/v1");

        await target.Run(destRoot, version, new("/Folder2/Folder1/*.txt"), "/Folder2");

        await VerifyDirectory(destRoot.FullName);
    }

    [Fact]
    public async Task GetByAnnotatedTag()
    {
        using var repo = TestRepository.OpenTestRepository();
        var version = new GitRef("tag/CheckoutAll");
        var destRoot = _outRoot.GetFolder("Tag0")
            .EnsureEmpty();

        var target = new GitGet.GitCommands.Get(repo);

        await target.Run(destRoot, version, GetFilter.All());

        await VerifyDirectory(destRoot.FullName);
    }

    [Fact]
    public async Task GetFiltered()
    {
        using var repo = TestRepository.OpenTestRepository();
        var commit = new GitRef("tag/v0");

        var glob = "Folder1/**/*.md,Folder2/**/*";

        var destRoot = _outRoot.GetFolder(nameof(GetFiltered))
            .EnsureEmpty();

        var target = new GitGet.GitCommands.Get(repo);

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

        var target = new GitGet.GitCommands.Get(repo);

        await target.Run(destRoot, commit, new(glob));

        await VerifyDirectory(destRoot.FullName);
    }
}