using Microsoft.Build.Framework;

namespace GitPackage.MSBuild;

public class UpdateTargetExists
{
    public UpdateTargetExists(){}

    /// <summary> </summary>
    [Required]
    public ITaskItem? Item { get; set; }

    [Output]
    public ITaskItem? Result { get; set; }
}
