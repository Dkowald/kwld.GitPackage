using GitGet.Actions;
using GitGet.Model;

using Microsoft.Extensions.Logging;

namespace GitGet.Tasks;

internal class Init : IAction
{
    private readonly ILogger _log;
    public Init(ILogger log)
    {
        _log = log;
    }

    public Task<int> Run(Args args)
    {
        //var statusFile = GitPackageStatusFile.Load(_log, args.TargetPath);

        return Task.FromResult(1);
    }
}