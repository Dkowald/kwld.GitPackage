//using System.IO.Abstractions.TestingHelpers;

//using GitPackage.Tests.App_Assets;
//using GitPackage.Tests.Util;

//namespace GitPackage.Tests.GlobCheckout;

//public class GlobCheckoutTests
//{
//  [Fact]
//  public void CheckoutAll()
//  {
//    var repo = TestRepository.OpenTestRepository();

//    var sourceRef = "refs/heads/master";

//    var filter = new GitGetFilter();

//    var outDir = new FileSystem().Project()
//      .GetFolder("App_Data", nameof(GlobCheckoutTests), nameof(CheckoutAll));

//    var outActual = outDir.GetFolder("actual");

//    var target = new GitPackage.GlobCheckout.GlobCheckout(repo);

//    target.Checkout(sourceRef, outActual, filter);

//    var expected = Assets.GlobCheckoutTests.CheckoutAll(new MockFileSystem().Current());

//    //var outExpected = outDir.FileSystem.Project()
//    //  .GetFolder("App_Assets", nameof(GlobCheckoutTests), nameof(CheckoutAll), "expected")
//    //  .EnsureExists();

//    AssertFiles.Same(expected, outActual, true);
//  }

//  [Fact]
//  public void CheckoutFiltered()
//  {
//    var repo = TestRepository.OpenTestRepository();

//    var target = new GitPackage.GlobCheckout.GlobCheckout(repo);

//    var actual = new FileSystem().Project().GetFolder("App_Data",
//      nameof(GlobCheckoutTests),
//      nameof(CheckoutFiltered), "actual");

//    var filter = new GlobFilterBuilder()
//      .Include("Folder2/**/*")
//      .Build();

//    target.Checkout("refs/tags/CheckoutAll", actual, filter);

//    var expected = Assets.GlobCheckoutTests.CheckoutFiltered(new MockFileSystem().Current());

//    AssertFiles.Same(expected, actual, true);
//  }
//}
