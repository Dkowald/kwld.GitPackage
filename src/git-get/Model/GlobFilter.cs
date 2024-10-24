using DotNet.Globbing;

namespace GitGet.Model;

internal class GlobFilter : IEquatable<GlobFilter>
{
    private static readonly GlobOptions Options;

    private readonly Glob[] _globs;

    public static readonly GlobFilter MatchAll;

    static GlobFilter()
    {
        Options = GlobOptions.Default;
        Options.Evaluation.CaseInsensitive = true;

        MatchAll = new("**/*");
    }

    public GlobFilter(string globs)
    {
        var parts = globs.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Order();

        _globs = parts.Select(Encode)
            .Select(x => Glob.Parse(x, Options))
            .ToArray();
    }

    public bool IsMatch(string path) =>
        _globs.Any(x => x.IsMatch(path));

    public override string ToString()
        => string.Join(',', _globs.Select(x => Decode(x.ToString())));

    #region Eqality
    public override bool Equals(object? obj)
        => obj is GlobFilter f && Equals(f);

    public bool Equals(GlobFilter? rhs)
    {
        if(rhs is null) return false;
        return ToString().Equals(rhs.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
        => ToString().ToLower().GetHashCode();

    public static bool operator ==(GlobFilter? lhs, GlobFilter? rhs)
        => lhs?.Equals(rhs) ?? rhs is null;

    public static bool operator !=(GlobFilter? lhs, GlobFilter? rhs)
        => !(lhs == rhs);

    #endregion

    private string Encode(string glob)
        => glob.StartsWith('/') || glob.StartsWith("**/")
            ? glob : $"**/{glob}";

    private static string Decode(string glob)
        => glob.StartsWith("**/") ? glob[3..] : glob;
}