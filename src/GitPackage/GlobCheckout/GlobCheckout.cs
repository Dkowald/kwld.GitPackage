using GitPackage.GlobCheckout.Error;
using LibGit2Sharp;

namespace GitPackage.GlobCheckout;

/// <summary>
/// Check-out a particular git repository commit
/// using a specified <see cref="GlobFilter"/>,
/// target folder, and commit.
/// </summary>
public class GlobCheckout
{
  private readonly Repository _repository;

  public GlobCheckout(Repository repository)
  {
    _repository = repository;
  }

  public void Checkout(string commit, IDirectoryInfo outFolder, GlobFilter filter)
  {
    var gitRef = _repository.Refs[commit]?.ResolveToDirectReference()
      ?? throw new ErrorNoSuchCommit(commit);

    var gitCommit = gitRef.Target as Commit;

    if (gitCommit is null && gitRef.Target is TagAnnotation tag)
    {
      gitCommit = tag.Target as Commit;
    }

    if(gitCommit is null)
      throw new ErrorRefIsNotACommit(commit);

    Checkout(gitCommit, outFolder, filter);
  }

  public void Checkout(Commit commit, IDirectoryInfo outFolder, GlobFilter filter)
  {
    var items = GlobTree(commit.Tree, filter).ToArray();
    
    foreach (var item in items)
    { Checkout(outFolder, item); }
  }

  internal IEnumerable<Entry> GlobTree(Tree root, GlobFilter filter)
  {
    var stack = new Stack<(Tree Tree, string ParentPath)>();
    stack.Push(new(root, ""));
    while (stack.Count > 0)
    {
      var current = stack.Pop();
      foreach (var item in current.Tree)
      {
        if (item.TargetType == TreeEntryTargetType.Tree)
        {
          var subTree = _repository.Lookup<Tree>(item.Target.Sha);
          var subPath = $"{current.ParentPath}{item.Path}/";
          stack.Push(new(subTree, subPath));
          continue;
        }

        var entry = new Entry(current.ParentPath, item);
        if (filter.IsMatch(entry.FullPath))
          yield return entry;
      }
    }
  }

  internal void Checkout(IDirectoryInfo root, Entry item)
  {
    var targetFile = root.GetFile(item.FullPath);
    var blob = item.Item.Target as Blob ??
               throw new Exception("can only checkout a blob");
    using var rd = blob.GetContentStream();
    using var wr = targetFile.EnsureDirectory().Create();
    rd.CopyTo(wr);
  }
}