namespace GitGet.Actions.Errors;

internal class ErrorCannotCleanTarget : Exception
{
    public ErrorCannotCleanTarget(string targetPath, Exception? inner = null)
        : base($"Destination cannot be cleaned: '{targetPath}'", inner)
    {
        TargetPath = targetPath;
    }

    public string TargetPath { get; }
}