namespace GitGet.Utility;

public static class DirectoryExtensions
{
    /// <summary>
    /// Clear read only attribute on ALL contained files.
    /// </summary>
    public static IDirectoryInfo ClearReadonly(this IDirectoryInfo target)
    {
        if (!target.Exists()) return target;

        var files = target.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (var item in files) { item.IsReadOnly = false; }

        return target;
    }

}