using Microsoft.Build.Framework;

namespace GitPackage.Tasks;

/// <summary>
/// If requested commit not found in cached git;
/// try refresh from origin
/// </summary>
public class FetchIfMissing
{
    [Required]
    public ITaskItem? Item { get; set; }
}