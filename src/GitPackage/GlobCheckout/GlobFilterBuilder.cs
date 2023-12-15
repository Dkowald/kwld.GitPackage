using DotNet.Globbing;

namespace GitPackage.GlobCheckout;

/// <summary>
/// Builder for <see cref="GlobFilter"/>.
/// </summary>
public class GlobFilterBuilder
{
  private readonly List<string> _include = [];
  private readonly List<string> _ignore = [];

  public GlobFilterBuilder Include(params string[] paths)
  {
    _include.AddRange(paths);
    return this;
  }

  public GlobFilterBuilder IncludeAll()
  {
    _include.Insert(0, "**/*");
    return this;
  }

  public GlobFilterBuilder Ignore(params string[] paths)
  {
    _ignore.AddRange(paths);
    return this;
  }

  public GlobFilter Build() =>
    new (
      _include.Select(Glob.Parse).ToArray(),
      _ignore.Select(Glob.Parse).ToArray());
}