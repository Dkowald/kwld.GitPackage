using GitGet.Actions;
using GitGet.Model;
using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace GitGet.Tests.Actions
{
    public class InfoTests
    {
        [Fact]
        public async Task ReviewCacheEntries()
        {
            var capture = new List<string>();
            var log = new FakeLogger(capture.Add);
            var con = new FakeConsole();

            var target = new Info(log, con);

            var args = Args.Load(new FileSystem(), log, LogLevel.Trace,
                [$"--cache:{Files.TestPackageCacheRoot.FullName}"]);

            Assert.NotNull(args);

            //put test repo in cache
            var cache = new RepositoryCache(log, args.Cache);
            var entry = cache.Get(TestRepository.BareRepoPath.AsUri());
            cache.CloneIfMissing(entry, null);

            Assert.NotNull(args);

            var exitCode = await target.Run(args);

            Assert.Equal(0, exitCode);
        }
    }
}
