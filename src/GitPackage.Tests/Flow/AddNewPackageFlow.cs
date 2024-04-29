using GitPackage.Tasks;
using GitPackage.Tests.TestHelpers;
using kwld.Xunit.Ordering;

namespace GitPackage.Tests.Flow;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class AddNewPackageFlow
{
    private IDirectoryInfo _code = new FileSystem().Project()
        .GetFolder("App_Data", "Flow", "AddNew");

    [Ordered, Fact]
    public void Setup() { }

    [Ordered, Fact]
    public void GetByTag()
    {
        var itemData = new StubTaskItem();
        var item = new GitPackageItem(itemData)
        {
            Include = TestRepository.Path.AsUri().ToString(),
            Path = _code.FullName
        };

        new Clone
        {
            Item = itemData
        }.Execute();
    }

    [Ordered, Fact]
    public void GetByBranch() { }

    [Ordered, Fact]
    public void UpdateOriginBranchAndTag() { }

    [Ordered, Fact]
    public void GetByNewerTag() { }

    [Ordered, Fact]
    public void GetByBranchHead() { }

    [Ordered, Fact]
    public void DisableFetchAbility()
    {
        //disable access to origin (so no fetch)
    }

    [Ordered, Fact]
    public void GetByTag_NoChange()
    {
    }

    [Ordered, Fact]
    public void GetByBranch_NoChange() { }
}