namespace GitPackage.Cli.Model;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// TODO: consider represent the status file as html,
/// so then I could just open it in browser to see info
/// </remarks>
internal record GitPackageStatusFile
{
    private readonly IFileInfo _statusFile;

    private GitPackageStatusFile(IFileInfo statusFile)
    {
        _statusFile = statusFile;
    }

    public static GitPackageStatusFile Load(IFileInfo dataFile)
    {
        var result = new GitPackageStatusFile(dataFile);

        if(dataFile.Exists)
            foreach (var line in dataFile.ReadAllLines())
            {
                var idx = line.IndexOf('=');
                if(idx < 0 || idx == line.Length-1)
                    throw new Exception("Data file corrupt");
                var key = line[..idx++].Trim();
                var value = line[idx..].Trim();
                
                if(key.Same(nameof(Include)))
                    result.Include = value;

                if (key.Same(nameof(Version)))
                    result.Version = new(value);

                if (key.Same(nameof(Filter)))
                    result.Filter = new(value);

                if (key.Same(nameof(Path)))
                    result.Path = new(value);
                
                if (key.Same(nameof(Commit)))
                    result.Commit = new(value);
            }

        //actually path will always be wherever the status file is.
        result.Path = dataFile.DirectoryName!;
        
        return result;
    }

    public void Write()
    {
        var content = new[]
        {
            $"{nameof(Include)} = {Include}",
            $"{nameof(Version)} = {Version}",
            $"{nameof(Filter)} = {Filter}",
            $"{nameof(Commit)} = {Commit}",
        };

        _statusFile.WriteAllLines(content);
    }

    /// <summary>
    /// Url to the source repository: e.g.
    /// https://github.com/Dkowald/kwld.Xunit.Ordering.git
    /// Note the leading https:// can be removed if desired
    /// </summary>
    public string Include { get; set; }

    /// <summary>
    /// The Version; corresponds to a git commit.
    /// Can be either a branch or a tag ref:
    /// tags/v1.0.0-preview1
    /// heads/dev
    /// If not prefixed with tags or heads; then it is assumed to be tags.
    /// </summary>
    public GitRef Version { get; set; }

    /// <summary>
    /// Optional glob filter on source files.
    /// </summary>
    public string Filter { get; set; }

    /// <summary>
    /// Local path to place files.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// The commit used for current files, if files have been collected
    /// </summary>
    public string? Commit { get; set; }
}