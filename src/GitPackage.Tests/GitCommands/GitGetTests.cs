using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitPackage.Tests.GitCommands
{
    public class GitGetTests
    {
        [Fact]
        public void Checkout()
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
