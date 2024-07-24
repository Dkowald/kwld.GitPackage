using System.Diagnostics;

using GitGet;

using GitPackage.Tests.TestHelpers;
using GitPackage.Tests.Util;

using Microsoft.Extensions.Configuration;

namespace GitPackage.Tests.Usage;

public class SecureRepository
{
    [Fact]
    public async Task Get() 
    {
        using var _ = Files.AppData.GetFolder(nameof(SecureRepository))
            .EnsureExists()
            .PushD();

        var cfg = new ConfigurationBuilder()
            .AddUserSecrets<SecureRepository>()
            .Build();

        var url = cfg["SecureRepository:Origin"];
        if (url is null)
        {
            Debug.Write("Skipping test: no secure repository defined in project secrets.");
            return;
        }
        
        var args = new string[] {
            $"--origin:{url}",
            $"--password:{cfg["SecureRepository:Password"]}",
            $"--user:{cfg["SecureRepository:User"]}",
            $"--version:branch/main",
            $"--filter:/readme.md",
            "--log-level:i"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
    }
}
