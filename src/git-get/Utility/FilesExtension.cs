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

    /// <summary>
    /// The util EnsureEmpty fails if the
    /// directory is in use. if is current directory.
    /// </summary>
    public static IDirectoryInfo EnsureEmptyWithoutDelete(this IDirectoryInfo folder)
    {
        if (!folder.Exists()) return folder.EnsureExists();

        foreach (var item in folder.EnumerateFileSystemInfos())
        {
            if(item is IDirectoryInfo dir)
                dir.Delete(true);
            else
                item.Delete();
        }

        return folder;
    }
}