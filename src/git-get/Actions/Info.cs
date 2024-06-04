using GitGet.Model;
using GitGet.Services;
using Microsoft.Extensions.Logging;

namespace GitGet.Actions
{
    internal class Info : IAction
    {
        private readonly ILogger _log;
        private readonly IConsole _console;

        public Info(ILogger log, IConsole console)
        {
            _log = log;
            _console = console;
        }

        public Task<int> Run(Args args)
        {
            if (!args.Cache.Exists())
            {

            }

            var cache = new RepositoryCache(_log, args.Cache);

            var items = cache.List();

            var entries = items.Select(x => new {
                x.Origin.Host,
                Path = x.Origin.AbsolutePath.Replace('\\', '/'),
                x.Origin,
                x.CachePath
            }).GroupBy(x => x.Host);

            foreach (var entry in entries) {

                var lines = new List<string>
                {
                    "-------------",
                    $"Host: {entry.Key}",
                };
                foreach (var item in entry)
                {
                    lines.Add($"Uri: {item.Origin}");
                    lines.Add($"Cache: {item.CachePath}");
                }

                foreach (var line in lines)
                { _console.Out.WriteLine(line); }
            }

            return Task.FromResult(0);
        }

    }
}
