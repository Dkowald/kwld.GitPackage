namespace GitPackage.Tests.Util
{
    internal static class DirectoryExtensions
    {
        public static IDisposable PushD(this IDirectoryInfo dir) =>
            new PushD(dir);
    }
}
