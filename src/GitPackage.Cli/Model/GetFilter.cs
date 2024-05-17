using DotNet.Globbing;

namespace GitPackage.Cli.Model;

/// <summary>
/// A set of glob filter(s) to select files.
/// The default is match all.
/// </summary>
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

        parts = parts.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if(parts.Length == 0)
        {_filters = [Glob.Parse("/*"),Glob.Parse("/**/*", options)];}
        else
        {
            _filters = parts.Select(x => Glob.Parse(
                x.StartsWith('/') ? x: $"/{x}"
            )).ToArray();
        }
    }

    public bool IsMatch(string path) =>
        _filters.Any(x => x.IsMatch(path));

    public override string ToString()
        => string.Join(';', _filters.Select(x => x.ToString()));
}