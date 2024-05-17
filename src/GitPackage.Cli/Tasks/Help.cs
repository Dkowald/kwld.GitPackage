using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Tasks
{
    internal class Help
    {
        private readonly ILogger _appLog;
        public Help(ILogger appLog)
        {
            _appLog = appLog;
        }
        public Task<int> Run()
        {
            _appLog.LogInformation("Version 0.0.1");

            return Task.FromResult(0);
        }
    }
}
