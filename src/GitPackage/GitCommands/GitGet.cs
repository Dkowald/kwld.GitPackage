using LibGit2Sharp;

namespace GitPackage.GitCommands;

/// <summary>
/// Get (rather than checkout) files for the repository,
/// </summary>
public class GitGet
{
    public GitGet(Repository repository){}
    
    public void Run(IDirectoryInfo target, string commit, string glob)
    {
        throw new NotImplementedException("todo");
    }
}