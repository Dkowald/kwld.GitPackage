using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GitPackage.Cli.Model;

/// <summary>
/// Present user input as valid git refs. <br/>
/// Cannot be empty. <br/>
/// Value can be a regular git-ref, or short-form <br/>
/// <list type="bullet">
/// <item>[tag]</item>
/// <item>tag/[tag]</item>
/// <item>branch/[branch]</item>
/// <item>refs/heads/[branch]</item>
/// <item>refs/tags/[tag]</item>
/// </list>
/// </summary>
/// <remarks>
/// Note: git refs are case preserving.
/// </remarks>
[JsonConverter(typeof(DataStringConverterFactory))]
internal record GitRef : IDataString<GitRef>
{
    private readonly string _prefix;
    private readonly string _path;

    private GitRef(string prefix, string path)
    {
        _prefix = prefix;
        _path = path;
    }

    public static (string? error, GitRef? value) TryRead(string data)
    {
        data = data.Trim();
        var parts = data.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return ($"{nameof(GitRef)} Cannot be empty", null);

        if (parts[0].Same("refs"))
        {
            if (parts.Length < 2)
                return ($"{nameof(GitRef)} invalid format, must contain a /", null);

            if (!parts[1].Same("heads") && !parts[1].Same("tags"))
                return ($"{nameof(GitRef)} ref must be either for head or tag", null);

            if (parts.Length < 3)
                return ($"{nameof(GitRef)} ref must include path to item", null);

            return (null, new($"{parts[0]}/{parts[1]}", string.Join('/', parts[2..])));
        }
         
        if (parts.Length == 1)
        {
            return (null, new("refs/tags", parts[0]));
        }

        var prefix = 
            parts[0].Same("branch") ?"refs/heads" :
            parts[0].Same("tag")? "refs/tags" : null;

        if(prefix is null)
            return ("short form must use branch or tag", null);

        var path = string.Join('/', parts[1..]);

        return (null, new(prefix, path));
    }

    /// <summary>Try parse version as a <see cref="GitRef"/> </summary>
    public static GitRef? TryParse(string? data)
    {
        if (data is null) return null;

        var (_, item) = TryRead(data);

        return item;
    }

    /// <inheritdoc cref="GitRef"/>
    public GitRef(string data)
    {
        var (error, item) = TryRead(data);

        if (error is not null)
            throw new ArgumentException(error, nameof(data));

        _prefix = item._prefix;
        _path = item._path;
    }

    public bool IsBranch => Value.StartsWith("refs/heads");
    public bool IsTag => Value.StartsWith("refs/tags");

    /// <summary>The full git ref string</summary>
    public string Value => $"{_prefix}/{_path}";

    /// <summary>
    /// Format for git package version value,
    /// <list type="bullet">
    ///  <item>branch/[branch]</item>
    ///  <item>tag/[tag]</item>
    /// </list>
    /// </summary>
    public string Version => $"{GitToShortForm}/{_path}";

    /// <inheritdoc cref="Value"/>
    [return:NotNullIfNotNull(nameof(GitRef))]
    public static implicit operator string?(GitRef? self) => self?.ToString();

    /// <inheritdoc cref="Value"/>
    public override string ToString() => Value;

    private string GitToShortForm =>
        _prefix.Same("refs/heads") ? "branch" : "tag";
}