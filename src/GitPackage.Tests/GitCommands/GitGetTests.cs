using GitPackage.GitCommands;
using LibGit2Sharp;

namespace GitPackage.Tests.GitCommands
{
    public class GitGetTests
    {
        private readonly IDirectoryInfo OutRoot = 
            new FileSystem().Project().GetFolder("App_Data", "GitGet");

        [Fact]
        public async Task Checkout()
        {
            var repo = TestRepository.OpenTestRepository();

            var commit = "refs/tags/v0";

            var info = repo.Refs[commit];
            var details =(Commit) info.ResolveToDirectReference().Target;
            var srcTree = details.Tree;

            var destRoot = new FileSystem().Project()
                .GetFolder("App_Data", "GitGet")
                .GetFolder("Tag0");

            foreach(var file in ReadTree(repo, srcTree))
            {
                if (file.Child.TargetType != TreeEntryTargetType.Blob)
                    throw new Exception("can only handle blobs.");

                var targetFile = destRoot.GetFile(file.FullPath[1..]);
                
                //todo: glob filter.

                var content = (Blob)file.Child.Target;
                using var rd = content.GetContentStream();
                using var wr = targetFile.EnsureDirectory().Create();
                rd.CopyTo(wr);
            }

        }

        [Fact]
        public async Task GetByTag()
        {
            var repo = TestRepository.OpenTestRepository();
            var commit = "refs/tags/v0";
            var destRoot = OutRoot.GetFolder("Tag0")
                .EnsureEmpty();

            var target = new GitGet(repo);

            target.Run(destRoot, commit);

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetByTag))
                .UseDirectory("Snapshot");
        }

        [Fact]
        public async Task GetFiltered()
        {
            var repo = TestRepository.OpenTestRepository();
            var commit = "refs/tags/v0";
            var glob = "Folder1/**/*;Folder2/**/*";
            var destRoot = OutRoot.GetFolder("Tag0Filtered")
                .EnsureEmpty();

            var target = new GitGet(repo);

            target.Run(destRoot, commit, glob);

            await VerifyDirectory(destRoot.FullName)
                .UseFileName(nameof(GetFiltered))
                .UseDirectory("Snapshot");
        }

        private IEnumerable<Entry> ReadTree(Repository repo, Tree root)
        {
            var stack = new Stack<(Tree Tree, string Name)>([(root, "")]);

            while (stack.Count > 0)
            {
                var next = stack.Pop();
                foreach (var item in next.Tree)
                {
                    if (item is null) continue;

                    if (item.TargetType == TreeEntryTargetType.Tree)
                    {
                        var subTree = repo.Lookup<Tree>(item.Target.Sha);
                        stack.Push((subTree, next.Name+"/"+item.Path));
                        continue;
                    }

                    yield return new(item.Name, item);
                }
            }
        }

        public record Entry(string Path, TreeEntry Child)
        {
            public string FullPath = Path + "/" + Child.Path;
        }
    }
}
