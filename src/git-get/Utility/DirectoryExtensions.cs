namespace GitGet.Utility;

internal static class DirectoryExtensions
{
    /// <summary>
    /// Clear read only attribute on ALL contained files.
    /// </summary>
    public static IDirectoryInfo ClearReadonly(this IDirectoryInfo target)
    {
        if(!target.Exists()) return target;

        var files = target.GetFiles("*.*", SearchOption.AllDirectories);
        foreach(var item in files) { item.IsReadOnly = false; }

        return target;
    }

    public static IDirectoryInfo MakeEmpty(this IDirectoryInfo target)
    {
        if(!target.Exists()) {
            target.Create();
            return target;
        }

        foreach(var item in target.EnumerateFileSystemInfos()) {
            if(item is IDirectoryInfo dir) { dir.EnsureDelete(); } else
                ((IFileInfo)item).Delete();
        }

        return target;
    }
}