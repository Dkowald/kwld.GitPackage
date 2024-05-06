using Microsoft.Build.Utilities;

namespace GitPackage
{
    /// <summary>
    /// todo: consider use this as wrapper for the cli.
    /// https://learn.microsoft.com/en-us/dotnet/api/microsoft.build.utilities.tooltask?view=msbuild-17-netcore
    /// </summary>
    public class GitPackageCli : ToolTask
    {
        protected override string GenerateFullPathToTool()
        {
            throw new NotImplementedException();
        }

        protected override string ToolName { get; }
    }
}
