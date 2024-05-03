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
internal class GitRef : IDataString<GitRef>
{
    private readonly string _prefix;
    private readonly string _path;

    private GitRef(string prefix, string path)
    {
        _prefix = prefix;
        _path = path;
    }

    private static (string? error, GitRef? value) TryRead(string data)
    {
        data = data.Trim();
        var parts = data.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return ("Cannot be empty", null);

        if (parts[0].Same("refs"))
        {
            if (parts.Length < 2)
                return ("Invalid git ref format", null);

            if (!parts[1].Same("heads") && !parts[1].Same("tags"))
                return ("Git ref must be either for head or tag", null);

            if (parts.Length < 3)
                return ("Git ref must include path to item", null);

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

    /// <inheritdoc cref="GitRef"/>
    public GitRef(string data)
    {
        var (error, item) = TryRead(data);

        if (error is not null)
            throw new ArgumentException(error, nameof(data));

        _prefix = item._prefix;
        _path = item._path;
    }

    /// <summary>Try parse version as a <see cref="GitRef"/> </summary>
    public static GitRef? TryParse(string? data)
    {
        if (data is null) return null;

        var (_, item) = TryRead(data);

        return item;
    }

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
    public override string ToString() => Value;

    private string GitToShortForm =>
        _prefix.Same("refs/heads") ? "branch" : "tag";
}