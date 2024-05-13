using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace GitPackage.Cli.Model;

internal class AppArgs
{
    public static (string? error, AppArgs? args) TryLoad(IConfiguration config)
    {
        var appArgs = new AppArgs
        {
            RepositoryCache = config[nameof(RepositoryCache)]!
        };

        var packages = config["Packages"]?.Trim();

        if (packages is null)
        {
            return ("No packages provided", null);
        }

        if (packages.StartsWith('['))
        {
            appArgs.Packages = JsonSerializer.Deserialize<GitPackageItem[]>(packages)
                               ?? [];
        }
        else
        {
            var item = JsonSerializer.Deserialize<GitPackageItem>(packages);
            if (item is not null)
                appArgs.Packages = [item];
        }

        return (null, appArgs);
    }

    public string RepositoryCache { get; set; }

    /// <summary>
    /// Json object or array of <see cref="GitPackageItem"/>
    /// </summary>
    public GitPackageItem[] Packages { get; set; }
}