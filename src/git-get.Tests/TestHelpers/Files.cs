namespace GitGet.Tests.TestHelpers;

internal static class Files
{
    public static readonly IDirectoryInfo AppData =
        new FileSystem().Project().GetFolder("App_Data");

    public static readonly IDirectoryInfo TestPackageCacheRoot =
        AppData.GetFolder(".gitpackages");
}