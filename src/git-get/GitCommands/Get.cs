using GitGet.Model;

using LibGit2Sharp;

namespace GitGet.GitCommands;

/// <summary>
/// Get (rather than checkout) from a repository.
/// </summary>
internal class Get(Repository repository, bool skipSubModules = true)
{
    /// <summary>
    /// <inheritdoc cref="Get"/>
    /// </summary>
    /// <param name="target">
    /// Folder withing the target git repository
    /// </param>
    /// <param name="commit">
    /// Commit to collect files from
    /// </param>
    /// <param name="include">
    /// Filter collected files, only extracting those that match.
    /// If null, all files in the subPath are included
    /// </param>
    /// <param name="ignore">
    /// Ignore selected files. If null, all included files included.
    /// </param>
    /// <param name="subPath">
    /// Optional sub-path within the repository to use as root path for get.
    /// </param>
    public async Task<GetResults> Run(IDirectoryInfo target, GitRef commit,
        GlobFilter? include = null, GlobFilter? ignore = null, RootPath? subPath = null)
    {
        include ??= GlobFilter.MatchAll;
        var commitRef = repository.Refs[commit.Value]
            ?.ResolveToDirectReference()
            ?.Target.Peel<Commit>()
            ?? throw new Exception($"Commit {commit} not found");

        var rootTree = TryFindRoot(commitRef.Tree, subPath);
        if(rootTree.Tree == null) {
            throw new Exception($"Subpath {subPath} not found");
        }

        var total = 0;
        var included = 0;
        var ignored = 0;
        foreach(var file in ReadTree(rootTree.Tree)) {

            if(skipSubModules && file.Item.TargetType == TreeEntryTargetType.GitLink)
            { continue; }

            if(file.Item.TargetType != TreeEntryTargetType.Blob)
                throw new Exception("can only handle blobs.");

            total++;
            var actualRepoPath = $"{subPath}{file.Path}";

            if(!include.IsMatch(actualRepoPath)) { continue; }
            included++;

            if(ignore?.IsMatch(actualRepoPath) == true) {
                ignored++;
                continue;
            }

            var content = (Blob)file.Item.Target;
            var outFile = target.GetFile( ((string)file.Path)[1..]);

            await using var rd = content.GetContentStream();
            await using var wr = outFile.EnsureDirectory().Create();
            await rd.CopyToAsync(wr);
        }

        return new(commitRef.Sha, total, included, ignored);
    }

    private record BlobEntry(RootPath Path, TreeEntry Item);

    private IEnumerable<BlobEntry> ReadTree(Tree root)
    {
        var stack = new Stack<(Tree Tree, string Path)>([(root, "")]);

        while(stack.Count > 0) {
            var next = stack.Pop();
            foreach(var item in next.Tree) {

                if(item.TargetType == TreeEntryTargetType.Tree) {
                    var subTree = repository.Lookup<Tree>(item.Target.Sha);
                    stack.Push((subTree, $"{next.Path}/{item.Name}"));
                    continue;
                }

                var path = $"{next.Path}/{item.Name}";

                yield return new(new(path), item);
            }
        }
    }

    private (Tree? Tree, string Path) TryFindRoot(Tree trueRoot, string? subPath)
    {
        if(string.IsNullOrEmpty(subPath)) return (trueRoot, "");

        var parts = subPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var current = trueRoot;
        var rootPath = "";
        foreach(var part in parts) {
            var entry = current
                .FirstOrDefault(x => x is not null && x.Name.Same(part) && x.TargetType == TreeEntryTargetType.Tree);

            if(entry == null) { current = null; break; }

            current = entry.Target != null ? repository.Lookup<Tree>(entry.Target.Sha) : null;
            rootPath = $"{rootPath}/{entry.Name}";

            if(current == null) break;
        }

        return (current, rootPath);

    }
}