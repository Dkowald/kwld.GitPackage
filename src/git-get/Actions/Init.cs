using GitGet.Model;

using Microsoft.Extensions.Logging;

namespace GitGet.Actions;

internal class Init : IAction
{
    private readonly ILogger _log;
    public Init(ILogger log)
    {
        _log = log;
    }

    public async Task<int> Run(Args args)
    {
        var (file, changed) = await StatusFile.TryLoadWithOverrides(_log, args);

        if(file == null) return 1;

        if(changed) {
            _log.LogInformation("Writing updated package file");
            await file.Write(_log);
        }

        return 0;
    }
}