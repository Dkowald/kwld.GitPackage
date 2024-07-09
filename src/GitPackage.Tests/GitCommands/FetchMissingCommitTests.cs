using GitGet.Utility;
using LibGit2Sharp;

namespace GitPackage.Tests.GitCommands;

public class FetchMissingCommitTests
{
    private IDirectoryInfo _origin;

    public FetchMissingCommitTests()
    {
        var root = new FileSystem().Project()
            .GetFolder("App_Data", "FetchMissing");

        _origin = root.GetFolder("origin")
            .ClearReadonly()
            .EnsureEmpty();
        
        CreateOrigin();
        
        var clone = root
            .GetFolder("clone")
            .ClearReadonly()
            .EnsureDelete();

        Repository.Clone(_origin.AsUri().ToString(), 
           clone.FullName, new CloneOptions{ IsBare = true });
    }

    private void CreateOrigin()
    {
        _origin.GetFile("File1").Touch();
        _origin.GetFile("File2.txt").WriteAllText("file 2");

        Repository.Init(_origin.FullName);
        var originRepo = new Repository(_origin.FullName);

        var sig = new Signature("test", "test", DateTimeOffset.Now);
        var commit = originRepo.Commit("Init", sig, sig);

        originRepo.Tags.Add("v0.0.1", commit, sig, "v0.0.1");
    }

    [Fact(Skip = "todo")]
    public void AlreadyHaveCommit()
    {
        
    }

    [Fact(Skip = "todo")]
    public void FoundDuringFetch(){}

    [Fact(Skip = "todo")]
    public void NotFoundDuringFetch(){}

    
}