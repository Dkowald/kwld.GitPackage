using System.Text.Json;

using GitGet.Model;

namespace GitGet.Tests.Model
{
    public class GitRefTests
    {
        [Fact]
        public void Serializable()
        {
            var data = new GitRef("v1");

            var json = JsonSerializer.Serialize(data);
            var reload = JsonSerializer.Deserialize<GitRef>(json);
            Assert.Equal(data, reload);
        }

        [Fact]
        public void Ctor_TagOnly()
        {
            var result = GitRef.TryParse("v2.0");
            Assert.NotNull(result);

            Assert.Equal("refs/tags/v2.0", result.Value);
        }

        [Fact]
        public void Ctor_ShortForm()
        {
            var result = GitRef.TryParse("branch/Wip");
            Assert.NotNull(result);

            Assert.Equal("refs/remotes/origin/Wip", result.Value);
        }

        [Fact]
        public void Ctor_GitForm()
        {
            var result = GitRef.TryParse("refs/remotes/origin/main");
            Assert.NotNull(result);

            Assert.Equal("refs/remotes/origin/main", result.Value);
            Assert.Equal("branch/main", result.Version);
        }

        [Fact]
        public void Ctor_InvalidForm()
        {
            try {
                _ = new GitRef("ref/heads/main");
                Assert.Fail("Should not create");
            } catch(ArgumentException) { }
        }
    }
}
