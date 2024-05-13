using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitPackage.Cli.Utility
{
    internal static class FilesExtension
    {
        public static IDirectoryInfo? TryGetHome(this IFileSystem fileSystem)
        {
            var home = Environment.GetEnvironmentVariable("HOME") ??
                       Environment.GetEnvironmentVariable("USERPROFILE");

            return home is null ? null : fileSystem.DirectoryInfo.New(home);
        }
    }
}
