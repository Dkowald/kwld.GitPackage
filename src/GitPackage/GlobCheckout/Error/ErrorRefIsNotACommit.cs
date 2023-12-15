namespace GitPackage.GlobCheckout.Error;

public class ErrorRefIsNotACommit : Exception
{
  public ErrorRefIsNotACommit(string commit, Exception? inner = null)
    : base($"'{commit}' is not a commit reference", inner)
  {
    Commit = commit;
  }

  public string Commit { get; }
}