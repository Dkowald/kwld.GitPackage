using Microsoft.Build.Framework;

namespace GitPackage.Tasks;

/// <summary>
/// Clone task item to local repository cache
/// </summary>
public class Clone : Microsoft.Build.Utilities.Task
{
    public override bool Execute()
    {
        throw new NotImplementedException();
    }
        
    /// <summary>
    /// Collection of GitPackage items to process.
    /// </summary>
    [Required]
    public ITaskItem? Item { get; set; }
}