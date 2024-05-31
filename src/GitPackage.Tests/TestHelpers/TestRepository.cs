using System.Diagnostics;

using LibGit2Sharp;

namespace GitPackage.Tests.TestHelpers;

static class TestRepository
{
    private static readonly IDirectoryInfo Path = new FileSystem()
        .Project().GetFolder("App_Data", "TestRepositoryWorking");

    private static readonly IDirectoryInfo BareRepoPath = new FileSystem()
        .Project().GetFolder("App_Data", "TestRepository");

    private static Signature Sig =>
        new ("test", "Test@com.au", DateTimeOffset.UtcNow);

    internal static Repository OpenTestRepository(bool forceRebuild = false)
    {
        if (!Repository.IsValid(BareRepoPath.FullName) || forceRebuild)
        {
            Debug.WriteLine("Creating test repository");
            BareRepoPath.ClearReadonly().EnsureEmpty();
            Path.ClearReadonly().EnsureEmpty();
            
            var repo = new Repository(Repository.Init(Path.FullName));

            Init(repo);
            UpdateDeleteAndMove(repo);
            IncludeNestedSameNameFolder(repo);
            Repository.Clone(Path.FullName, BareRepoPath.FullName, new()
            {
                IsBare = true,
                FetchOptions = { TagFetchMode = TagFetchMode.All}
            });
        }

        if (!Repository.IsValid(BareRepoPath.FullName))
            throw new Exception("Create test repository failed");

        return new Repository(BareRepoPath.FullName);
    }

    private static void Init(Repository repo)
    {
        Path.GetFile("readme.md")
            .WriteAllText("readme.md");
        repo.Index.Add("readme.md");

        Path.GetFile("item0.txt")
            .WriteAllText("item0.txt");
        repo.Index.Add("item0.txt");

        Path.GetFile("Folder1/item1.txt")
            .EnsureDirectory()
            .WriteAllText("item1.txt");
        repo.Index.Add("Folder1/item1.txt");

        Path.GetFile("Folder2/item2.txt")
            .EnsureDirectory()
            .WriteAllText("item2.txt");
        repo.Index.Add("Folder2/item2.txt");
        
        repo.Index.Write();

        repo.Commit("Init", Sig, Sig);
        repo.ApplyTag("v0");
    }

    private static void UpdateDeleteAndMove(Repository repo)
    {
        Path.GetFile("readme.md")
            .WriteAllText("Updated");
        repo.Index.Add("readme.md");

        Path.GetFile("item0.txt").Delete();
        repo.Index.Remove("item0.txt");

        var moveTo = Path.GetFile("folder2/item1.txt");
        Path.GetFile("Folder1/item1.txt").MoveTo(moveTo);

        repo.Index.Remove("Folder1/item1.txt");
        repo.Index.Add("folder2/item1.txt");
        repo.Index.Write();

        repo.Commit("update-del-move", Sig, Sig);

        repo.ApplyTag("CheckoutAll", Sig, "CheckoutAll");
    }

    private static void IncludeNestedSameNameFolder(Repository repo)
    {
        Commands.Checkout(repo, "refs/tags/v0");

        Commands.Checkout(repo, 
        repo.CreateBranch(nameof(IncludeNestedSameNameFolder)));
        
        var f = Path.GetFile("Folder2", "Folder1", "nested.txt");
        f.EnsureDirectory().WriteAllText("nested.txt");
        
        repo.Index.Add(f.GetRelativePath(Path));

        repo.Index.Write();
        repo.Commit(nameof(IncludeNestedSameNameFolder), Sig, Sig);
        repo.ApplyTag("v1");
    }
}