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
            ReportPackageInfo(args);
            _console.Out.WriteLine("---");

            ReportCacheInfo(args);

            return Task.FromResult(0);
        }

        private void ReportPackageInfo(Args args)
        {
            var statusFile = StatusFile.LoadIfFound(_log, args.TargetPath);

            _console.Out.WriteLine("## Status file details");
            _console.Out.WriteLine($"Location: {args.TargetPath.FullName}");
            
            if (statusFile is not null)
            {
                _console.Out.WriteLine($"  Origin:  '{statusFile.Origin}'");
                _console.Out.WriteLine($"  Version: '{statusFile.Version}'");
                _console.Out.WriteLine($"  Filter:  '{statusFile.Filter}'");
                _console.Out.WriteLine($"  Commit:  '{statusFile.Commit}'");
            }
            else
            {
                _console.Out.WriteLine($"Status file '{StatusFile.StatusFolder}/{StatusFile.ConfigFile}' not found");
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

            foreach (var entry in entries)
            {
                var lines = new List<string>
                {
                    "---",
                    $"Host: {entry.Key}",
                };
                foreach (var item in entry)
                {
                    lines.Add($"  Uri: {item.Origin}");
                    lines.Add($"  Cache: {item.CachePath}");
                }

                foreach (var line in lines)
                { _console.Out.WriteLine(line); }
            }

        }
    }
}
