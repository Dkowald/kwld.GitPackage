namespace GitGet.Model;

/// <summary>
/// Combines an include and optional ignore filter
/// to filter git get results.
/// </summary>
internal class GetFilter : IEquatable<GetFilter>
{
    private readonly GlobFilter _include;
    private readonly GlobFilter? _ignore;

    /// <summary>
    /// Create filter to match all; with optional ignore.
    /// </summary>
    /// <returns></returns>
    public static GetFilter All(string? ignore = null)
        => new(GlobFilter.MatchAll, ignore == null ? null : new(ignore));
    
    public GetFilter(string include, string? ignore = null):
        this(new(include), ignore is null? null : new GlobFilter(ignore)){}

    public GetFilter(GlobFilter include, GlobFilter? ignore = null)
    {
        _include = include;
        _ignore = ignore;
    }

    public bool IsMatch(string path)=>
        _include.IsMatch(path) && 
        _ignore?.IsMatch(path) != true;

    #region Equal
    public override bool Equals(object? obj) =>
        obj is GetFilter f && Equals(f);

    public bool Equals(GetFilter? rhs) {
        if (rhs is null) return false;

        return _include == rhs._include &&
               _ignore == rhs._ignore;
    }

    public override int GetHashCode() =>
        HashCode.Combine(_include, _ignore);

    public static bool operator ==(GetFilter? lhs, GetFilter? rhs)
        => lhs?.Equals(rhs) ?? rhs is null;

    public static bool operator !=(GetFilter? lhs, GetFilter? rhs) => !(lhs == rhs);
    #endregion
}