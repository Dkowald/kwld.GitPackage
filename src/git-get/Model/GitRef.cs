using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GitGet.Model;

/// <summary>
/// Present user input as valid git refs.<br/>
/// Cannot be empty. <br/>
/// Value can be a regular full-form git-ref, or short-form <br/>
/// Must have origin remote for branches (full-form)<br/>
/// <list type="bullet">
/// <item>[tag]</item>
/// <item>tag/[tag]</item>
/// <item>branch/[branch]</item>
/// <item>refs/remotes/origin/[branch]</item>
/// <item>refs/tags/[tag]</item>
/// </list>
/// </summary>
/// <remarks>
/// Note: git refs are case preserving.
/// </remarks>
[JsonConverter(typeof(DataStringConverterFactory))]
internal record GitRef : IDataString<GitRef>
{
    private const string BranchRefPrefix = "refs/remotes/origin";
    private const string TagRefPrefix = "refs/tags";

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

        //explicit ref.
        if(data.StartsWith("refs")) {
            var isBranchRef = data.StartsWith(BranchRefPrefix, StringComparison.OrdinalIgnoreCase);
            var isTagRef = data.StartsWith(TagRefPrefix, StringComparison.OrdinalIgnoreCase);

            if(!isBranchRef && !isTagRef)
                return ($"{nameof(GitRef)} explicit ref must be either branch ('{BranchRefPrefix}/[branch]') or tag '{TagRefPrefix}/[tag]' ", null);

            if(isBranchRef) {
                if(data.Same(BranchRefPrefix))
                    return ($"{nameof(GitRef)} explicit branch ref must include path to branch", null);
                return (null, new(BranchRefPrefix, data[(BranchRefPrefix.Length + 1)..]));
            }

            if(isTagRef) {
                if(data.Same(TagRefPrefix))
                    return ($"{nameof(GitRef)} explicit tag ref must include path to branch", null);
                return (null, new(TagRefPrefix, data[(TagRefPrefix.Length + 1)..]));
            }
        }

        //short-form

        var parts = data.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if(parts.Length == 0) return ($"{nameof(GitRef)} Cannot be empty", null);

        if(parts[0].Same("refs")) {
            return (null, new($"{parts[0]}/{parts[1]}", string.Join('/', parts[2..])));
        }

        if(parts.Length == 1) {
            return (null, new(TagRefPrefix, parts[0]));
        }

        var prefix =
            parts[0].Same("branch") ? BranchRefPrefix :
            parts[0].Same("tag") ? TagRefPrefix : null;

        if(prefix is null)
            return ("short form must use branch or tag", null);

        var path = string.Join('/', parts[1..]);

        return (null, new(prefix, path));
    }

    /// <summary>Try parse version as a <see cref="GitRef"/> </summary>
    public static GitRef? TryParse(string? data)
    {
        if(data is null) return null;

        var (_, item) = TryRead(data);

        return item;
    }

    /// <inheritdoc cref="GitRef"/>
    public GitRef(string data)
    {
        var (error, item) = TryRead(data);

        if(item is null)
            throw new ArgumentException(error ?? "Invalid format", nameof(data));

        _prefix = item._prefix;
        _path = item._path;
    }

    public bool IsBranch => Value.StartsWith(BranchRefPrefix);
    public bool IsTag => Value.StartsWith(TagRefPrefix);

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
    [return: NotNullIfNotNull(nameof(GitRef))]
    public static implicit operator string?(GitRef? self) => self?.ToString();

    /// <inheritdoc cref="Version"/>
    public override string ToString() => Version;

    private string GitToShortForm =>
        _prefix.Same(BranchRefPrefix) ? "branch" : "tag";
}