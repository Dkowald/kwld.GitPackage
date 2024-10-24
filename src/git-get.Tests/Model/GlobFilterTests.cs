using GitGet.Model;

namespace GitGet.Tests.Model;

public class GlobFilterTests
{
    public static TheoryData<string?, string, bool> Data => new() {
        //edge
        {"/readme.md,deploy/**/*.yaml", "data/deploy/objects/README.md", false},

        //non anchored
        {"readme.md", "/readme.md", true},
        {"readme.md", "/other/readme.md", true},
        {"notFound.txt", "/other/readme.md", false},

        //case ignorant
        {"readme.md", "/ReadME.Md", true},
        {"/OTHER/readme.md", "/Other/README.md", true},

        //root only
        {"/*.md", "/README.md", true},
        {"/*.md", "/doc/README.md", false},
        
        //deep
        {"*.md", "/README.md", true},
        {"*.md", "/doc/README.md", true},
        {"*.md", "/doc/README.txt", false},

        {"other/**/*.md", "/doc/other/deep/README.md", true},
        {"other/**/*.md", "/doc/deep/README.txt", false},

        //one of
        {"/*.md", "/doc/other/deep/README.md", false},
        {"deep/*.md", "/doc/other/deep/README.md", true},
        {"/*.md,deep/*.md", "/doc/other/deep/README.md", true},
        
        //all
        {null, "/x.md", true},
        {null, "/docs/other/x.md", true},
        {"**/*", "/docs/other/x.md", true}
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void MatchCheck(string? pattern, string input, bool isMatch)
    {
        var target = pattern is null ? GlobFilter.MatchAll :
            new GlobFilter(pattern);

        Assert.Equal(isMatch, target.IsMatch(input));

        var txt = target.ToString();
        Assert.NotNull(txt);
        var reload = new GlobFilter(txt);

        Assert.Equal(isMatch, reload.IsMatch(input));
    }
}