using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Task = Microsoft.Build.Utilities.Task;

namespace GitPackage
{
    /// <summary>
    /// Convert a GitPackage TaskItem to a corresponding
    /// GitGet json file.
    /// If task item data differs from existing, it is overwritten.
    /// </summary>
    public class GitPackageTask : Task
    {
        /// <summary>
        /// Collection of GitPackage items to process.
        /// </summary>
        [Required]
        public ITaskItem Package { get; set; }

        [Output] public ITaskItem Item { get; set; }

        public override bool Execute()
        {
            var packageTaskItem = new GitPackageTaskItem(Package);

            var outFile = Path.Combine(packageTaskItem.Path, ".gitpackage");

            var current = File.Exists(outFile) ? 
                File.ReadAllLines(outFile): Array.Empty<string>();

            var expected = packageTaskItem.AsLines();

            var isMatch = current.Length >= expected.Length;

            if(isMatch)
                for (var idx = 0; idx < expected.Length; idx++)
                {
                    if (expected[idx] != current[idx])
                    {
                        isMatch = false;
                        break;
                    }
                }

            if (!isMatch)
            {
                Directory.CreateDirectory(packageTaskItem.Path);
                File.WriteAllLines(outFile, expected);
            }

            Item = new TaskItem(outFile);
            
            return true;
        }

        internal string[] SplitPropertyLine(string line)
        {
            var idx = line.IndexOf('=');
            if (idx < 0) return new [] { null, line };

            return idx > 0 && idx < line.Length
                ? new[]
                {
                    line.Substring(0, idx),
                    line.Substring(idx)
                }
                : new[] { null, line };
        }
    }
}