using Microsoft.Extensions.Logging.Testing;

using GitGet.Actions;
using GitPackage.Tests.TestHelpers;
using GitGet.Model;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.Actions
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
            cache.CloneIfMissing(entry);

            Assert.NotNull(args);

            var exitCode = await target.Run(args);

            Assert.Equal(0, exitCode);
        }
    }
}
