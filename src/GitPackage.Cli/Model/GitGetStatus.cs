using System.Text.Json;

namespace GitPackage.Cli.Model;

/// <summary>
/// Verifies the folder matches expected 
/// </summary>
internal class GitGetStatus
{
    internal const string StatusFileName = ".gitpackage";

    private readonly IFileInfo _statusFile;

    private readonly Content _expected;

    public GitGetStatus(IDirectoryInfo target, string repositoryUrl , GitRef commit)
    {
        _statusFile = target.GetFile(StatusFileName);

        _expected = new(repositoryUrl, commit.Value);
    }

    public bool IsMatch()
    {
        if (!_statusFile.Exists) return false;

        var current = TryLoadStatusFile();
        if (current is null) return false;

        return current == _expected;
    }

    public Task SetMatched()=> 
        JsonSerializer.SerializeAsync(
            _statusFile.EnsureDirectory().Create(),
            _expected);

    private Content? TryLoadStatusFile()
    {
        try
        {
            return JsonSerializer.Deserialize<Content>(_statusFile.ReadAllText());
        }
        catch
        {
            _statusFile.Delete();
        }

        return null;
    }

    private record Content(string Url, string Commit);
}