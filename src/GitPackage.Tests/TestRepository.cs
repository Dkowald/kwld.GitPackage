using LibGit2Sharp;

namespace GitPackage.Tests;

static class TestRepository
{
  internal static Repository OpenTestRepository()
  {
    var path = new FileSystem().Project()
      .GetFolder("App_Data", "TestRepository");

    if (!path.Exists)
      throw new Exception("Execute 'create-test-repository.ps1' before attempting to use");

    return new Repository(path.FullName);
  }
}