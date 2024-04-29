using kwld.Xunit.Ordering;

namespace GitPackage.Tests.Flow;

public class CheckForUpdatesFlow
{
    [Ordered, Fact]
    public void CreateOriginRepo() { }

    [Ordered, Fact]
    public void AddGitPackageForOrigin() { }

    [Ordered, Fact]
    public void UpdateOriginRepo() { }

    [Ordered, Fact]
    public void CheckForUpdate() { }
}