namespace GitPackage.Tests.Util;

public static class AssertFiles
{
  /// <summary>
  /// Asserts <see cref="actual"/> contains all the same files as <see cref="expected"/>.
  /// </summary>
  /// <param name="expected">The expected folder structure</param>
  /// <param name="actual">The actual folder structure</param>
  /// <param name="checkContent">True if content of each file must also match.</param>
  public static void Same(IDirectoryInfo expected, IDirectoryInfo actual, bool checkContent = true)
  {
    var (added, _, deleted) = expected.TreeCUD(actual);

    Assert.False(added.Any(), "Additional files found");
    Assert.False(deleted.Any(), "Missing files in target");

    if (checkContent)
    {
      foreach (var src in expected.EnumerateFiles("*", SearchOption.AllDirectories))
      {
        var relPath = src.GetRelativePath(expected);
        var dest = actual.GetFile(relPath);
        var sameFile = src.ReadAllBytes().SequenceEqual(dest.ReadAllBytes());
        Assert.True(sameFile);
      }
    }
  }
}