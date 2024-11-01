using GitGet.Model;
using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.Model;

public class RepositoryCacheTests
{
    [Fact]
    public void Purge_()
    {
        var localCache = Files.AppData.GetFolder(nameof(RepositoryCacheTests));

        localCache.EnsureEmpty();


        var target = new RepositoryCache(new FakeLogger(), localCache);

        var origin = TestRepository.BareRepoPath.AsUri();
        var cacheEntry = target.Get(origin);

        target.CloneIfMissing(origin, null).Dispose();
        
        Assert.True(cacheEntry.CachePath.Exists());

        target.Purge(origin);

        Assert.False(cacheEntry.CachePath.Exists());
    }

    [Fact]
    public void CloneIfMissing_()
    {
        var cache = Files.AppData.GetFolder(nameof(RepositoryCache), "cache")
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

        //make sure I have at-least the test repo.
        target.CloneIfMissing(cacheTest, null).Dispose();
        
        var result = target.List().ToArray();

        Assert.NotEmpty(result);
    }
}
