namespace GitPackage.Tests.TestHelpers;

internal static class Files
{
    public static IDirectoryInfo TestPackageCacheRoot =
        new FileSystem().Project()
            .GetFolder("App_Data", ".gitpackages");
}