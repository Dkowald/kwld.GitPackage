using DotNet.Globbing;
using GitPackage.Cli.Model;

namespace GitPackage.Tests.Model;

public class GetFilterTests
{
    [Fact]
    public void CaseIgnorant()
    {
        var target = new GetFilter("/readme.md");
        var result = target.IsMatch("/README.md");

        Assert.True(result);
    }
}