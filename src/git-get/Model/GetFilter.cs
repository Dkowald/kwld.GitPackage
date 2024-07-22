using DotNet.Globbing;

namespace GitPackage.Cli.Model;

/// <summary>
/// A set of glob filter(s) to select files.
/// The default is match all.
/// </summary>
/// <remarks>
/// Consider making this behave like gitignore
/// https://git-scm.com/docs/gitignore?ref=linuxandubuntu.com#_pattern_format
/// </remarks>
internal class GetFilter : IEquatable<GetFilter>
{
    public const string NoFilter = "**/*";

    private readonly Glob[] _filters;

    /// <summary>
    /// Create glob filter, defaults to no filter glob.
    /// </summary>
    /// <param name="globs">series of comma seperated globs</param>
    public GetFilter(string globs = NoFilter)
    {
        var options = GlobOptions.Default;
        options.Evaluation.CaseInsensitive = true;

        var parts = globs.Split(',', StringSplitOptions.RemoveEmptyEntries);

        parts = parts.Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(Encode)
            .ToArray();

        _filters = parts.Length == 0 ?
            [Glob.Parse("**/*")] :
            parts.Select(Glob.Parse).ToArray();
    }

    public bool IsMatch(string path) =>
        _filters.Any(x => x.IsMatch(path));

    public override string ToString()
        => string.Join(',', _filters.Select(x => Decode(x.ToString())));

    #region Equal
    public override bool Equals(object? obj) =>
        obj is GetFilter f && Equals(f);

    public bool Equals(GetFilter? rhs) {
        if (rhs is null) return false;

        var lhsFilters = _filters.Select(x => x.ToString())
            .Order().ToArray();

        var rhsFilters = rhs._filters.Select(x => x.ToString())
            .Order().ToArray();

        return lhsFilters.SequenceEqual(rhsFilters);
    }

    public override int GetHashCode() =>
        _filters.Aggregate(0, (v, x) => HashCode.Combine(v, x.ToString()));

    public static bool operator ==(GetFilter? lhs, GetFilter? rhs)
        => lhs is not null ? lhs.Equals(rhs) : rhs is null;

    public static bool operator !=(GetFilter? lhs, GetFilter? rhs) => !(lhs == rhs);
    #endregion

    private string Encode(string glob)
        => glob.StartsWith('/') || glob.StartsWith("**/")
            ? glob : $"**/{glob}";

    private string Decode(string glob)
        => glob.StartsWith("**/") ? glob[3..] : glob;
}