using GitGet.Model;

namespace GitPackage.Tests.Model;

public class GetFilterTests
{
    [Fact]
    public void FilterIgnored()
    {
        var target = new GetFilter("*.txt", "test*.txt");

        Assert.True(target.IsMatch("info.txt"));

        Assert.False(target.IsMatch("test-info.txt"));
    }
}