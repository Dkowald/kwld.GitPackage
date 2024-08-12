using GitGet.Model;
using GitGet.Services;

using InMemLogger;

using Microsoft.Extensions.Logging;

namespace GitGet.Actions
{
    internal class Where : IAction
    {
        private readonly ILogger _log;
        private readonly IConsole _console;

        public Where(ILogger log, IConsole console)
        {
            _log = log;
            _console = console;
        }

        public async Task<int> Run(Args args)
        {
            if(args.Origin is null) {
                _log.LogError("Missing target repository origin");
                return 1;
            }

            //TODO: swap to NullLogger for .net 9
            var noLogging = new InMemoryLogger();

            var cache = new RepositoryCache(noLogging, args.Cache);

            var result = cache.Get(args.Origin);
            await _console.Out.WriteAsync(result.CachePath.FullName);

            return 0;
        }
    }
}
