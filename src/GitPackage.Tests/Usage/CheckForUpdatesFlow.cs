using GitGet.Utility;

using GitPackage.Tests.TestHelpers;

using LibGit2Sharp;

namespace GitPackage.Tests.Usage;

public class CheckForUpdatesFlow
{
    private readonly IDirectoryInfo Root = Files.AppData.GetFolder(nameof(CheckForUpdatesFlow));
    private readonly Signature Sig = new("test", "Test@com.au", DateTimeOffset.UtcNow);

    [Ordered, Fact]
    public void CreateOriginRepo() 
    {
        var origin = Root.GetFolder("Origin")
            .ClearReadonly().EnsureEmpty();

        Repository.Init(origin.FullName);
        using var repo = new Repository(origin.FullName);

        repo.CreateBranch("main");

        origin.GetFile("Readme.md").WriteAllLines(["Initial commit"]);
        repo.Index.Add("Readme.md");
        repo.Index.Write();
        repo.Commit("Init", Sig, Sig);
    }

    [Ordered, Fact]
    public void AddGitPackageForOrigin() { }

    [Ordered, Fact]
    public void UpdateOriginRepo() 
    {
        var origin = Root.GetFolder("Origin");

        origin.GetFile("Readme.md").AppendAllLines(["Updated"]);

        using var repo = new Repository(origin.FullName);
        
        repo.Index.Add("Readme.md");
        repo.Index.Write();
        repo.Commit("Updated", Sig, Sig);
    }

    [Ordered, Fact]
    public void CheckForUpdate() { }
}