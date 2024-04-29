using LibGit2Sharp;

namespace GitPackage.Tests;

static class TestRepository
{
    internal static readonly IDirectoryInfo Path = new FileSystem()
        .Project().GetFolder("App_Data", "TestRepository");

    internal static Repository OpenTestRepository()
    {
        if (!Path.Exists)
            throw new Exception("Execute 'create-test-repository.ps1' before attempting to use");

        return new Repository(Path.FullName);
    }
}