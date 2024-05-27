using GitPackage.Cli.Model;

using LibGit2Sharp;

namespace GitPackage.Cli.GitCommands;

/// <summary>
/// Get (rather than checkout) files for the repository.
/// </summary>
internal class GitGet(Repository repository)
{
    public void Run(IDirectoryInfo target, GitRef commit, GetFilter filter)
    {
        var commitRef = repository.Refs[commit.Value]
            ?.ResolveToDirectReference()
            ?.Target.Peel<Commit>()
            ?? throw new Exception($"Commit {commit} not found");

        var rootTree = commitRef.Tree;

        foreach (var file in ReadTree(rootTree, filter)) {

            if (file.Item.TargetType != TreeEntryTargetType.Blob)
                throw new Exception("can only handle blobs(not sure about link.");
            
            var content = (Blob)file.Item.Target;
            var outFile = target.GetFile(file.Path[1..]);

            using var rd = content.GetContentStream();
            using var wr = outFile.EnsureDirectory().Create();
            rd.CopyTo(wr); //todo: async it.
        }
    }

    private record BlobEntry(string Path, TreeEntry Item);

    private IEnumerable<BlobEntry> ReadTree(Tree root, GetFilter filters)
    {
        var stack = new Stack<(Tree Tree, string Path)>([(root, "")]);

        while(stack.Count > 0)
        {
            var next = stack.Pop();
            foreach(var item in next.Tree) { 
            
                if(item.TargetType == TreeEntryTargetType.Tree)
                {
                    var subTree = repository.Lookup<Tree>(item.Target.Sha);
                    stack.Push((subTree, $"{next.Path}/{item.Name}"));
                    continue;
                }

                var path = $"{next.Path}/{item.Name}";
                
                if(filters.IsMatch(path))
                    yield return new(path, item);
            }
        }
    }
}