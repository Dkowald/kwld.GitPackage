using GitGet.Model;
using GitGet.Tests.TestHelpers;
using GitGet.Utility;

using LibGit2Sharp;

using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.Model;

public class RepositoryCacheTests
{
    [Fact]
    public void CloneIfMissing_()
    {
        var cache = Files.AppData.GetFolder(nameof(RepositoryCache), "cache")
            .ClearReadonly()
            .EnsureEmpty();

        var logs = new List<string>();
        var log = new FakeLogger(logs.Add);
        var target = new RepositoryCache(log, cache);

        var url = TestRepository.BareRepoPath.AsUri();

        target.CloneIfMissing(url, null);

        var serverWork = logs.Where(x => x.Contains("Fetching"));
        Assert.True(serverWork.Any());
    }

    [Fact]
    public void Get_LocalRepo()
    {
        var log = new FakeLogger();
        var files = new FileSystem();

        var target = new RepositoryCache(log, files.Current());

        var result = target.Get(new(Files.TestPackageCacheRoot.FullName));

        var expected = files.Current()
            .GetFolder(RepositoryCache.LocalHostPath)
            .FullName;

        Assert.StartsWith(expected, result.CachePath.FullName);
    }

    [Fact]
    public void List_()
    {
        //ensure have test repo.
        using var testRepo = TestRepository.OpenTestRepository();

        var log = new FakeLogger();
        var target = new RepositoryCache(log, Files.TestPackageCacheRoot);

        var cacheTest = target.Get(TestRepository.BareRepoPath.AsUri());

        if(!cacheTest.CachePath.Exists) {
            //make sure i have at-least the test repo.
            Repository.Clone(
                TestRepository.BareRepoPath.AsUri().ToString(),
                cacheTest.CachePath.FullName, new() { IsBare = true });
        }

        var result = target.List().ToArray();

        Assert.NotEmpty(result);
    }
}
