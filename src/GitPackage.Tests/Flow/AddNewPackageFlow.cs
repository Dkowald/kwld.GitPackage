using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using kwld.Xunit.Ordering;

namespace GitPackage.Tests.Flow;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class AddNewPackageFlow
{
    private static readonly IDirectoryInfo AppData =  new FileSystem().Project()
        .GetFolder("App_Data");

    [Ordered, Fact]
    public async Task GetByTag()
    {
        var item = new GitPackageItem
        {
            Include = TestRepository.Path.AsUri().ToString(),
            Path = AppData.GetFolder("Flow", nameof(GetByTag)).FullName,
            Version = new("tag/CheckoutAll")// "tags/v0"
        };
        
        var cfg = new AppConfig
        {
            RepositoryCache = Files.TestPackageCacheRoot.FullName
        };

        var result = await Program.Run(cfg, item);

        Assert.Equal(0, result);
    }
}