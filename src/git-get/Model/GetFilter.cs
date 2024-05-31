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
internal class GetFilter
{
    public const string NoFilter = "**/*";

    private readonly Glob[] _filters;

    /// <summary>
    /// Create glob filter, defaults to no filter glob.
    /// </summary>
    public GetFilter(string globs = NoFilter)
    {
        var options = GlobOptions.Default;
        options.Evaluation.CaseInsensitive = true;
        
        var parts = globs.Split(';', StringSplitOptions.RemoveEmptyEntries);

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
        => string.Join(';', _filters.Select( x => Decode(x.ToString())));

    private string Encode(string glob)
        => glob.StartsWith('/') || glob.StartsWith("**/") 
            ? glob : $"**/{glob}";

    private string Decode(string glob)
        => glob.StartsWith("**/") ? glob[3..] : glob;
}