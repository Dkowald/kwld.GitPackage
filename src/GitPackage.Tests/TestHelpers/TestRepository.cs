using System.Diagnostics;

using LibGit2Sharp;

namespace GitPackage.Tests.TestHelpers;

static class TestRepository
{
    private static readonly IDirectoryInfo Path = new FileSystem()
        .Project().GetFolder("App_Data", "TestRepository2");

    internal static Repository OpenTestRepository(bool forceRebuild = false)
    {
        if (!Repository.IsValid(Path.FullName) || forceRebuild)
        {
            Debug.WriteLine("Creating test repository");
            Path.ClearReadonly().Delete(true);
            var repo = new Repository(Repository.Init(Path.FullName));

            Init(repo);
            UpdateDeleteAndMove(repo);
        }

        if (!Repository.IsValid(Path.FullName))
            throw new Exception("Create test repository failed");

        return new Repository(Path.FullName);
    }

    internal static readonly string TestRepositoryUrl = Path.FullName;

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

        var sig = new Signature("test", "Test@com.au", DateTimeOffset.UtcNow);

        repo.Commit("Init", sig, sig);
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

        var sig = new Signature("test", "Test@com.au", DateTimeOffset.UtcNow);

        repo.Commit("update-del-move", sig, sig);

        repo.ApplyTag("CheckoutAll", sig, "CheckoutAll");
    }
}