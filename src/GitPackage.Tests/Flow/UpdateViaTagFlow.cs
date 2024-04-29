using kwld.Xunit.Ordering;

namespace GitPackage.Tests.Flow;

public class UpdateViaTagFlow
{
    [Ordered, Fact]
    public void CreateOrignWithTag1(){}

    [Ordered, Fact]
    public void GetPackageWithTag1(){}

    [Ordered, Fact]
    public void UpdateOriginWithTag2(){}

    [Ordered, Fact]
    public void UpdatePackageWithTag2(){}
}