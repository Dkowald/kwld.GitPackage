namespace GitPackage.Tests;

public class ToDo
{
    [Fact(Skip = "Todo")]
    public void GetWithSubModule()
    {
        //Support using a git repo as a 
        // pinned package repository.
        
        //create sub-repo.
        //add sub repo to main, for specific sub-repo tag
        //gitget should include sub-repo files.
    }
    
    [Fact]
    public void CloneWithStructure()
    {
        //set the gitPackage cache root.
        //add gitPackage.
        //verify default is git
    }

    [Fact]
    public void RepoContainsVersionFile()
    {
        //if the repo include the
        //verison file used by this system.
        //then error.
    }
}