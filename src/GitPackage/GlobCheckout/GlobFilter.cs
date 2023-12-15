using DotNet.Globbing;

namespace GitPackage.GlobCheckout;

/// <summary>
/// Describes a glob-based filter to include, and
/// subsequently exclude paths.
/// </summary>
/// <param name="include">Set of include glob paths</param>
/// <param name="ignore">Set of ignore glob paths,
/// only ignores items that have been included.
/// </param>
public class GlobFilter
{
  private readonly Glob[] _include;
  private readonly Glob[] _ignore;

  public GlobFilter(Glob[] include, Glob[] ignore)
  {
    _include = include;
    _ignore = ignore;
  }

  /// <summary>
  /// Filter to include all file paths.
  /// </summary>
  public static readonly GlobFilter All = new
  ([Glob.Parse("**/*")], []);

  /// <summary>
  /// Returns true if the given <paramref name="path"/>
  /// is included; and is NOT matched by ignore.
  /// </summary>
  public bool IsMatch(string path)
  {
    if (!_include.Any(x => x.IsMatch(path)))
      return false;

    if (_ignore.Any(x => x.IsMatch(path)))
      return false;

    return true;
  }
}