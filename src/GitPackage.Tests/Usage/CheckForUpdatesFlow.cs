using GitGet;
using GitGet.Model;
using GitGet.Utility;

using GitPackage.Tests.TestHelpers;

using LibGit2Sharp;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class CheckForUpdatesFlow
{
    private readonly IDirectoryInfo Root = Files.AppData.GetFolder("Usage", nameof(CheckForUpdatesFlow));
    private readonly Signature Sig = new("test", "Test@com.au", DateTimeOffset.UtcNow);

    readonly IDirectoryInfo Origin;
    readonly IDirectoryInfo Cache;
    readonly IDirectoryInfo Working;

    public CheckForUpdatesFlow()
    {
        Origin = Root.GetFolder("Origin");

        Cache = Root.GetFolder(RepositoryCache.DefaultCacheFolderName);

        Working = Root.GetFolder("Working").EnsureExists();
    }

    [Ordered, Fact]
    public void Reset()
    {
        Origin.ClearReadonly().EnsureEmpty();
        Cache.ClearReadonly().EnsureDelete();
        Working.EnsureDelete();
    }

    [Ordered, Fact]
    public void CreateOriginRepo() 
    {
        Repository.Init(Origin.FullName);
        using var repo = new Repository(Origin.FullName);

        Origin.GetFile("Readme.md").WriteAllLines(["Initial commit"]);
        repo.Index.Add("Readme.md");
        repo.Index.Write();

        repo.Commit("Init", Sig, Sig);
    }

    [Ordered, Fact]
    public async Task GetOriginal() 
    {   
        using var _ = new PushD(Working);

        var args = new[]
        {
            $"--cache:{Cache.FullName}",

            $"--origin:{Origin.AsUri()}",
            $"--version:branch/master",
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);

        await VerifyDirectory(Working.FullName, pattern: "*.md");
    }

    [Ordered, Fact]
    public void UpdateOriginRepo() 
    {
        Origin.GetFile("Readme.md").AppendAllLines(["Updated"]);

        using var repo = new Repository(Origin.FullName);
        
        repo.Index.Add("Readme.md");
        repo.Index.Write();
        repo.Commit("Updated", Sig, Sig);
    }

    [Ordered, Fact]
    public async Task GetWithoutUptate()
    {
        using var _ = new PushD(Working);

        var args = new[]
        {
            $"--cache:{Cache.FullName}",
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);

        await VerifyDirectory(Working.FullName, pattern: "*.md")
            .UseMethodName(nameof(GetOriginal));
    }

    [Ordered, Fact]
    public async Task GetWithUpdate()
    {
        using var _ = new PushD(Working);

        var args = new[]
        {
            $"--cache:{Cache.FullName}",
            "--force:all"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);

        await VerifyDirectory(Working.FullName, pattern: "*.md");
    }
}