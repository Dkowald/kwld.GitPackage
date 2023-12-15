using LibGit2Sharp;

namespace GitPackage.GlobCheckout;

internal record Entry(string Path, TreeEntry Item)
{
  public string FullPath => $"{Path}{Item.Path}";
}