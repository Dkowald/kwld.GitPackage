using DotNet.Globbing;

namespace GitPackage.Cli.Model;

/// <summary>
/// A set of glob filter(s) to select files.
/// The default is match all.
/// </summary>
internal class GetFilter
{
    private readonly Glob[] _filters;

    public GetFilter(string? globs = null)
    {
        var parts = globs is not null ? globs.Split(';', StringSplitOptions.RemoveEmptyEntries) : [];

        parts = parts.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        if(parts.Length == 0)
        {_filters = [Glob.Parse("/*"),Glob.Parse("/**/*")];}
        else
        {
            _filters = parts.Select(x => Glob.Parse(
                x.StartsWith('/') ? x: $"/{x}"
            )).ToArray();
        }
    }

    public bool IsMatch(string path) =>
        _filters.Any(x => x.IsMatch(path));
}