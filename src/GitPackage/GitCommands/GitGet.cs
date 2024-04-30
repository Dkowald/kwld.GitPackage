using DotNet.Globbing;
using LibGit2Sharp;

namespace GitPackage.GitCommands;

/// <summary>
/// Get (rather than checkout) files for the repository,
/// </summary>
public class GitGet(Repository repository)
{
    public void Run(IDirectoryInfo target, string commit, string? glob = null)
    {
        var globs = !glob.IsNullOrEmpty() ?
                glob!.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Glob.Parse)
                    .ToArray() : [Glob.Parse("**/*")];

        var commitRef = repository.Refs[commit]
            ?.ResolveToDirectReference()
            ?.Target as Commit
            ?? throw new Exception($"Commit {commit} not found");

        var rootTree = commitRef.Tree;

        foreach (var file in ReadTree(rootTree, globs)) {

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

    private IEnumerable<BlobEntry> ReadTree(Tree root, Glob[] filters)
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
                
                if(filters.Any(x => x.IsMatch(path)))
                    yield return new(path, item);
            }
        }
    }
}