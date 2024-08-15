using System.Diagnostics;

using GitGet.Tests.TestHelpers;

using Microsoft.Extensions.Configuration;

namespace GitGet.Tests.Usage;

public class SecureRepository
{
    [Fact]
    public async Task Get()
    {
        var dir = Files.AppData.GetFolder(nameof(SecureRepository))
            .EnsureExists();

        var cfg = new ConfigurationBuilder()
            .AddUserSecrets<SecureRepository>()
            .Build();

        var url = cfg["SecureRepository:Origin"];
        if(url is null) {
            Debug.Write("Skipping test: no secure repository defined in project secrets.");
            return;
        }

        var args = new[] {
            dir.FullName,
            $"--origin:{url}",
            $"--password:{cfg["SecureRepository:Password"]}",
            $"--user:{cfg["SecureRepository:User"]}",
            "--version:branch/main",
            "--filter:/readme.md",
            "--log-level:i"
        };

        var exitCode = await Program.Main(args);

        Assert.Equal(0, exitCode);
    }
}
