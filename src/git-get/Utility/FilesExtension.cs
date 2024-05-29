using System.IO.IsolatedStorage;

namespace GitGet.Utility;

internal static class FilesExtension
{
    public static IDirectoryInfo? TryGetHome(this IFileSystem fileSystem)
    {
        var home = Environment.GetEnvironmentVariable("HOME") ??
                   Environment.GetEnvironmentVariable("USERPROFILE");

        return home is null ? null : fileSystem.DirectoryInfo.New(home);
    }

    public static bool IsFile(this IDirectoryInfo root, string path)
        => root.GetFile(path).Exists;
}