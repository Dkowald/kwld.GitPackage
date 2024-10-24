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

        public async Task<int> Run(Args args)
        {
            await ReportPackageInfo(args);
            await _console.Out.WriteLineAsync("---");

            ReportCacheInfo(args);

            return 0;
        }

        private async Task ReportPackageInfo(Args args)
        {
            var statusFile = await StatusFile.TryLoad(_log, args.TargetPath);

            await _console.Out.WriteLineAsync("## Status file details");
            await _console.Out.WriteLineAsync($"Location: {args.TargetPath.FullName}");

            if(statusFile is not null) {
                await _console.Out.WriteLineAsync($"  Origin:  '{statusFile.Origin}'");
                await _console.Out.WriteLineAsync($"  Version: '{statusFile.Version}'");
                await _console.Out.WriteLineAsync($"  Filter:  '{statusFile.Filter}'");
                await _console.Out.WriteLineAsync($"  GetRoot:  '{statusFile.GetRoot}'");
                await _console.Out.WriteLineAsync($"  Commit:  '{statusFile.Commit}'");
            } else {
                await _console.Out.WriteLineAsync($"Status file '{StatusFile.FileName}' not found");
            }
        }

        private void ReportCacheInfo(Args args)
        {
            var cache = new RepositoryCache(_log, args.Cache);

            var items = cache.List();

            var entries = items.Select(x => new {
                x.Origin.Host,
                Path = x.Origin.AbsolutePath.Replace('\\', '/'),
                x.Origin,
                x.CachePath
            }).GroupBy(x => x.Host);

            _console.Out.WriteLine("## Cache details");
            _console.Out.WriteLine($"Location: {args.Cache}");

            foreach(var entry in entries) {
                var lines = new List<string>
                {
                    "---",
                    $"Host: {entry.Key}",
                };
                foreach(var item in entry) {
                    lines.Add($"  Uri: {item.Origin}");
                    lines.Add($"  Cache: {item.CachePath}");
                }

                foreach(var line in lines) { _console.Out.WriteLine(line); }
            }

        }
    }
}
