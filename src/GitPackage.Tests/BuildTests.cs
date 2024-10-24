namespace GitPackage.Tests;

public class BuildTests
{
    [Fact]
    public void HasGitGetFiles()
    {
        var credits = new FileSystem().Project()
            .GetFolder("App_Data")
            .GetFolder("credits");

        var hasFiles = credits.EnumerateFiles("*", SearchOption.AllDirectories).Any();

        Assert.True(hasFiles);
    }
}