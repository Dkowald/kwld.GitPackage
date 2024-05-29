namespace GitPackage.Tests.TestHelpers;

public static class DirectoryExtensions
{
    /// <summary>
    /// Set attribute on ALL contained files
    /// </summary>
    public static IDirectoryInfo ClearReadonly(this IDirectoryInfo target)
    {
        if (!target.Exists()) return target;

        var files = target.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (var item in files) { item .IsReadOnly = false; }

        return target;
    }

}