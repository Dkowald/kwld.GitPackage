using System.IO.Abstractions.TestingHelpers;
using GitPackage.Tests.App_Assets;
using GitPackage.Tests.Util;
using LibGit2Sharp;

namespace GitPackage.Tests.DiffCheckout;

public class DiffCheckoutTests
{
  [Fact]
  public void ListBranches()
  {
    
    var expected = Assets.DiffCheckoutTests.LatestChangesInBranch(new MockFileSystem().Current());

    var f1 = expected.GetFile("readme.md");
    var content = f1.ReadAllText();

    using var repo = TestRepository.OpenTestRepository();

    var names = repo.Branches.Select(x => x.CanonicalName);

    var tags = repo.Tags.Select(x => x.CanonicalName);
  }
  
  [Fact]
  public void LatestChangesInBranch()
  {
    using var repo = TestRepository.OpenTestRepository();
    
    var sourceRef = "refs/heads/master";

    var actual = new FileSystem().Project()
      .GetFolder("App_Data", nameof(DiffCheckoutTests), nameof(LatestChangesInBranch), "actual")
      .EnsureEmpty();

    var target = new GitPackage.DiffCheckout.DiffCheckout(repo);

    target.Checkout(sourceRef, actual);

    var expected = Assets.DiffCheckoutTests.LatestChangesInBranch(new MockFileSystem().Current());

    AssertFiles.Same(expected, actual);
  }
}