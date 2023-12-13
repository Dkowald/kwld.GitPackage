
using System.IO.Abstractions;
using System.Linq;

using LibGit2Sharp;

namespace GitPackage.Tests.DiffCheckout;

public class DiffCheckoutTests
{
  private Repository GetRepo() =>
    new("C:\\Source\\Temp\\TestRepository");
    //new ("C:\\Source\\kwd\\kwd.kBox");
    //new("C:\\Source\\kwld\\kwld.CoreUtil");


  [Fact]
  public void ListBranches()
  {
    using var repo = GetRepo();

    var names = repo.Branches.Select(x => x.CanonicalName);

    var tags = repo.Tags.Select(x => x.CanonicalName);
  }
  
  [Fact]
  public void Run()
  {
    using var repo = GetRepo();

    var sourceRef = "refs/heads/master";
    //var sourceRef = "refs/tags/v1.3.1";
    var gitRef = repo.Refs[sourceRef]?.ResolveToDirectReference()
      ?? throw new Exception("Invalid ref");

    var sourceCommit = gitRef.Target as Commit 
      ?? throw new Exception("Ref must have a commit target");

    var oldCommit = sourceCommit.Parents.First();

    var diff = repo.Diff.Compare<TreeChanges>(oldCommit.Tree, sourceCommit.Tree);

    var diffItems = diff.Select(x =>
    {
      //if (x.Status == ChangeKind.Renamed)
      //{
      //  return new ChangeEntry[]
      //  {
      //    new (ChangeKind.Deleted, x.OldPath),
      //    new (ChangeKind.Added, x.Path)
      //  };
      //}

      if (x.Status == ChangeKind.Deleted)
      {
        return new[] { new ChangeEntry(ChangeKind.Deleted, x.OldPath, null) };
      }

      return new[] { new ChangeEntry(x.Status, x.Path, repo.Lookup(x.Oid)) };
    }).SelectMany(i => i)
    .ToArray();

    //todo: GitGlob the diffs.

    //checkout.
    var outDir = new FileSystem().Project().GetFolder("App_Data", nameof(DiffCheckoutTests));

    outDir.EnsureEmpty();
    foreach (var item in diffItems)
    {
      var outFile = outDir.GetFile(item.Path)
        .EnsureDirectory();
      if (item.Change == ChangeKind.Deleted)
        outFile = outFile.ChangeExtension($"deleted.{outFile.Extension}");

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
}