using GitPackage.GitCommands.Errors;
using LibGit2Sharp;

namespace GitPackage.GitCommands;

internal class ResolveReferenceCommit
{
  private readonly Repository _repository;

  public ResolveReferenceCommit(Repository repository)
  {
    _repository = repository;
  }

  public (Commit?, Exception? error) Try(string commit)
  {
    var gitRef = _repository.Refs[commit]?.ResolveToDirectReference();
    if (gitRef is null)
      return (null, new ErrorNoSuchCommit(commit));

    var gitCommit = gitRef.Target as Commit;

    if (gitCommit is null && gitRef.Target is TagAnnotation tag)
    {
      gitCommit = tag.Target as Commit;
    }

    if (gitCommit is null)
      return (null, new ErrorRefIsNotACommit(commit));

    return (gitCommit, null);
  }

  public Commit Resolve(string commit)
  {
    var (result, error) = Try(commit);

    if (result is null)
      throw error ?? new Exception("Failed to retrieve commit from reference");

    return result;
  }
}