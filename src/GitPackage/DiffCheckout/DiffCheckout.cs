using GitPackage.GitCommands;
using GitPackage.GitCommands.Model;
using LibGit2Sharp;

namespace GitPackage.DiffCheckout;

public class DiffCheckout
{
  private readonly Repository _repository;

  public DiffCheckout(Repository repository)
  {
    _repository = repository;
  }

  public void Checkout(string commit, IDirectoryInfo outDir)
  {
    var gitCommit = new FetchMissingCommit(_repository)
      .Resolve(commit);

    var parentCommit = gitCommit.Parents?.First();

    if (parentCommit is null)
    {
      //all items are add.
      throw new NotImplementedException();
    }
    
    var diff = _repository.Diff.Compare<TreeChanges>(parentCommit.Tree, gitCommit.Tree);

    var diffItems = diff.Select(x => 
      {
        if (x.Status == ChangeKind.Deleted)
        {
          return new[] { new ChangeEntry(ChangeKind.Deleted, x.OldPath, null) };
        }

        return new[] { new ChangeEntry(x.Status, x.Path, _repository.Lookup(x.Oid)) };
      }).SelectMany(i => i)
      .ToArray();

    outDir.EnsureEmpty();

    foreach (var item in diffItems)
    {
      Checkout(item, outDir);
    }
  }

  internal void Checkout(ChangeEntry item, IDirectoryInfo outDir)
  {
    var outFile = outDir.GetFile(item.Path)
      .EnsureDirectory();
    if (item.Change == ChangeKind.Deleted)
      outFile = outFile.ChangeExtension($"deleted{outFile.Extension}");

    if (item.Item is Blob content)
    {
      using var rd = content.GetContentStream();
      using var wr = outFile.Create();
      rd.CopyTo(wr);
    }
    else
    {
      outFile.Touch();
    }
  }
}