using GitGet.Model;

namespace GitGet.Tests.Model
{
    public class RootPathTests
    {
        [Fact]
        public void Ctor_AutoPrefix()
        {
            var autoFixed = new RootPath("apath");

            var expected = RootPath.TryParse("/apath");

            Assert.Equal(expected, autoFixed);
        }
    }
}
