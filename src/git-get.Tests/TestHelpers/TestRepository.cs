using System.Diagnostics;

using GitGet.Model;
using GitGet.Utility;

using LibGit2Sharp;

namespace GitGet.Tests.TestHelpers;

internal static class TestRepository
{
    private static readonly IDirectoryInfo WorkingFolderPath = new FileSystem()
        .Project().GetFolder("App_Data", "TestRepositoryWorking");

    private static readonly IDirectoryInfo RepoPath = new FileSystem()
        .Project().GetFolder("App_Data", "TestRepository");

    internal static IDirectoryInfo BareRepoPath {
        get {
            if(!Repository.IsValid(RepoPath.FullName))
                OpenTestRepository().Dispose();
            return RepoPath;
        }
    }

    private static Signature Sig =>
        new("test", "Test@com.au", DateTimeOffset.UtcNow);

    internal static Repository OpenTestRepository()
    {
        if(!Repository.IsValid(RepoPath.FullName)) {
            Debug.WriteLine("Creating test repository");
            
            if(Repository.IsValid(RepoPath.FullName)) 
            { return new Repository(RepoPath.FullName); }

            RepoPath.ClearReadonly().EnsureEmpty();
            WorkingFolderPath.ClearReadonly().EnsureEmpty();

            using(var repo = new Repository(Repository.Init(WorkingFolderPath.FullName))) {
                Init(repo);
                UpdateDeleteAndMove(repo);
                IncludeNestedSameNameFolder(repo);
                BranchHasStatusFile(repo);
                ManyFiles(repo);
            }

            var bare = WorkingFolderPath.GetFolder(".git");

            bare.MergeForce(RepoPath);
            using var bareRepo = new Repository(RepoPath.FullName);
            bareRepo.Config.Set("core.bare", true);

            WorkingFolderPath.ClearReadonly().EnsureDelete();

        }

        if(!Repository.IsValid(RepoPath.FullName))
            throw new Exception("Create test repository failed");

        var result = new Repository(RepoPath.FullName);

        return result;
    }

    private static void Init(Repository repo)
    {
        WorkingFolderPath.GetFile("readme.md")
            .WriteAllText("readme.md");
        repo.Index.Add("readme.md");

        WorkingFolderPath.GetFile("item0.txt")
            .WriteAllText("item0.txt");
        repo.Index.Add("item0.txt");

        WorkingFolderPath.GetFile("Folder1/item1.txt")
            .EnsureDirectory()
            .WriteAllText("item1.txt");
        repo.Index.Add("Folder1/item1.txt");

        WorkingFolderPath.GetFile("Folder2/item2.txt")
            .EnsureDirectory()
            .WriteAllText("item2.txt");
        repo.Index.Add("Folder2/item2.txt");

        repo.Index.Write();

        repo.Commit("Init", Sig, Sig);
        repo.ApplyTag("v0");
    }

    private static void UpdateDeleteAndMove(Repository repo)
    {
        WorkingFolderPath.GetFile("readme.md")
            .WriteAllText("Updated");
        repo.Index.Add("readme.md");

        WorkingFolderPath.GetFile("item0.txt").Delete();
        repo.Index.Remove("item0.txt");

        var moveTo = WorkingFolderPath.GetFile("Folder2/item1.txt");
        WorkingFolderPath.GetFile("Folder1/item1.txt").MoveTo(moveTo);

        repo.Index.Remove("Folder1/item1.txt");
        repo.Index.Add("Folder2/item1.txt");
        repo.Index.Write();

        repo.Commit("update-del-move", Sig, Sig);

        repo.ApplyTag("CheckoutAll", Sig, "CheckoutAll");
    }

    private static void IncludeNestedSameNameFolder(Repository repo)
    {
        Commands.Checkout(repo, "refs/tags/v0");

        Commands.Checkout(repo,
        repo.CreateBranch(nameof(IncludeNestedSameNameFolder)));

        var f = WorkingFolderPath.GetFile("Folder2", "Folder1", "nested.txt");
        f.EnsureDirectory().WriteAllText("nested.txt");

        repo.Index.Add(f.GetRelativePath(WorkingFolderPath));

        repo.Index.Write();
        repo.Commit(nameof(IncludeNestedSameNameFolder), Sig, Sig);
        repo.ApplyTag("v1");
    }

    private static void BranchHasStatusFile(Repository repo)
    {
        Commands.Checkout(repo, repo.Branches["master"]);
        var newBranch = repo.CreateBranch(nameof(BranchHasStatusFile));
        Commands.Checkout(repo, newBranch);

        WorkingFolderPath.GetFile(StatusFile.FileName)
            .WriteAllText("some data");
        repo.Index.Add(StatusFile.FileName);

        repo.Index.Write();

        repo.Commit("add status file", Sig, Sig);
    }

    private static void ManyFiles(Repository repo)
    {
        Commands.Checkout(repo, repo.Branches[nameof(BranchHasStatusFile)]);
        var newBranch = repo.CreateBranch(nameof(ManyFiles));
        Commands.Checkout(repo, newBranch);

        var manyFiles = WorkingFolderPath.GetFolder("manyFiles").EnsureExists();

        var file = manyFiles.GetFile("d0.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("d1.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("f0.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("f1.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("f2.x.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("Dir1", "f3-dir1.txt").EnsureDirectory();
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("Dir1", "f4-dir1.x.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        file = manyFiles.GetFile("Dir1", "f5-dir1.txt");
        file.WriteAllText(file.Name);
        repo.Index.Add(file.GetRelativePath(WorkingFolderPath));

        repo.Index.Write();

        repo.Commit("Many files", Sig, Sig);
    }
}