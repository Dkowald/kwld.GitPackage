using System.Collections;
using GitGet.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

using GitPackage.Cli.Model;

namespace GitPackage.Tests.Model;

public class GetFilterTests
{
    public record TestCases(string? Pattern, string Input, bool IsMatch);

    public static readonly IEnumerable<object[]> Data = new TestCases[]
    {
        //case ignorant
        new("readme.md", "ReadME.Md", true),
        new("/OTHER/readme.md", "/Other/README.md", true),

        //root only
        new("/*.md", "/README.md", true),
        new("/*.md", "/doc/README.md", false),
        
        //deep
        new("**/*.md", "/README.md", true),
        new("**/*.md", "/doc/README.md", true),
        new("**/*.md", "/doc/README.txt", false),

        new("**/other/**/*.md", "/doc/other/deep/README.md", true),
        new("**/other/**/*.md", "/doc/deep/README.txt", false),

        //all
        new(null, "x.md", true),
        new(null, "docs/other/x.md", true),
        new("**/*", "docs/other/x.md", true)

    }.Select(x => new[] { (object)x });

    [Theory]
    [MemberData(nameof(Data))]
    public void MatchCheck(TestCases data)
    {
        var target = data.Pattern is null ? 
            new GetFilter() : 
            new GetFilter(data.Pattern);

        Assert.Equal(data.IsMatch, target.IsMatch(data.Input));

        var txt = target.ToString();
        var reload = new GetFilter(txt);

        Assert.Equal(data.IsMatch, reload.IsMatch(data.Input));
    }
}