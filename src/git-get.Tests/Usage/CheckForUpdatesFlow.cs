using GitGet.Model;
using GitGet.Tests.TestHelpers;

using LibGit2Sharp;

namespace GitGet.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class CheckForUpdatesFlow
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Usage", nameof(CheckForUpdatesFlow));
    private readonly Signature _sig = new("test", "Test@com.au", DateTimeOffset.UtcNow);
    private readonly IDirectoryInfo _origin;
    private readonly IDirectoryInfo _cache;
    private readonly IDirectoryInfo _working;

    public CheckForUpdatesFlow()
    {
        _origin = _root.GetFolder("Origin");

        _cache = _root.GetFolder(RepositoryCache.DefaultCacheFolderName);

        _working = _root.GetFolder("Working").EnsureExists();
    }

    [Ordered, Fact]
    public void Reset()
    {
        _origin.EnsureEmpty();
        _cache.EnsureDelete();
        _working.EnsureDelete();
    }

    [Ordered, Fact]
    public void CreateOriginRepo()
    {
        Repository.Init(_origin.FullName);
        using var repo = new Repository(_origin.FullName);

        _origin.GetFile("Readme.md").WriteAllLines(["Initial commit"]);
        repo.Index.Add("Readme.md");
        repo.Index.Write();

        repo.Commit("Init", _sig, _sig);
    }

    [Ordered, Fact]
    public async Task GetOriginal()
    {
        var args = new[]
        {
            $"{_working.FullName}",
            $"--cache:{_cache.FullName}",

            $"--origin:{_origin.AsUri()}",
            "--version:branch/master"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);

        await VerifyDirectory(_working.FullName, pattern: "*.md");
    }

    [Ordered, Fact]
    public void UpdateOriginRepo()
    {
        _origin.GetFile("Readme.md").AppendAllLines(["Updated"]);

        using var repo = new Repository(_origin.FullName);

        repo.Index.Add("Readme.md");
        repo.Index.Write();
        repo.Commit("Updated", _sig, _sig);
    }

    [Ordered, Fact]
    public async Task GetWithoutUptate()
    {
        var args = new[]
        {
            _working.FullName,
            $"--cache:{_cache.FullName}",
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);

        await VerifyDirectory(_working.FullName, pattern: "*.md")
            .UseMethodName(nameof(GetOriginal));
    }

    [Ordered, Fact]
    public async Task GetWithUpdate()
    {
        var args = new[]
        {
            _working.FullName,
            $"--cache:{_cache.FullName}",
            "--force:all"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);

        await VerifyDirectory(_working.FullName, pattern: "*.md");
    }
}