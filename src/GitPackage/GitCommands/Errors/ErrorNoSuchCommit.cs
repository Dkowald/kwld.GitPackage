namespace GitPackage.GitCommands.Errors;

public class ErrorNoSuchCommit : Exception
{
  public ErrorNoSuchCommit(string commit, Exception? innerException = null)
    :base(
      $"Repository does not contain the commit: '{commit}'",
      innerException)
  { Attempted = commit; }

  public string Attempted { get; }
}