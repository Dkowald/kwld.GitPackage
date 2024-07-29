using GitPackage.Cli.Model;

using LibGit2Sharp;

namespace GitGet.GitCommands;

/// <summary>
/// Get (rather than checkout) files for the repository.
/// </summary>
internal class Get(Repository repository)
{
    public async Task<(string Commit, int Extracted)>  Run(IDirectoryInfo target, GitRef commit, 
        GetFilter filter, string? subPath = null)
    {
        var commitRef = repository.Refs[commit.Value]
            ?.ResolveToDirectReference()
            ?.Target.Peel<Commit>()
            ?? throw new Exception($"Commit {commit} not found");

        var rootTree = TryFindRoot(commitRef.Tree, subPath);
        if(rootTree == null)
        {
            throw new Exception($"Subpath {subPath} not found");
        }

        int count = 0;
        foreach (var file in ReadTree(rootTree, filter))
        {
            if (file.Item.TargetType != TreeEntryTargetType.Blob)
                throw new Exception("can only handle blobs.");

            var content = (Blob)file.Item.Target;
            var outFile = target.GetFile(file.Path[1..]);

            using var rd = content.GetContentStream();
            using var wr = outFile.EnsureDirectory().Create();
            await rd.CopyToAsync(wr);
            count++;
        }

        return (commitRef.Sha, count);
    }

    private record BlobEntry(string Path, TreeEntry Item);

    private IEnumerable<BlobEntry> ReadTree(Tree root, GetFilter filters)
    {
        var stack = new Stack<(Tree Tree, string Path)>([(root, "")]);

        while (stack.Count > 0)
        {
            var next = stack.Pop();
            foreach (var item in next.Tree)
            {

                if (item.TargetType == TreeEntryTargetType.Tree)
                {
                    var subTree = repository.Lookup<Tree>(item.Target.Sha);
                    stack.Push((subTree, $"{next.Path}/{item.Name}"));
                    continue;
                }

                var path = $"{next.Path}/{item.Name}";

                if (filters.IsMatch(path))
                    yield return new(path, item);
            }
        }
    }

    private Tree? TryFindRoot(Tree trueRoot, string? subPath) 
    {
        if (string.IsNullOrEmpty(subPath)) return trueRoot;

        var parts = subPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var current = trueRoot;
        foreach(var part in parts)
        {
            var entry = current.OfType<TreeEntry>()
                .FirstOrDefault(x => x.Name.Same(part));

            current = entry?.Target != null ?repository.Lookup<Tree>(entry.Target.Sha) : null;

            if (current == null) break;
        }
        
        return current;

    }
}