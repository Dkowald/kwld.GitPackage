using System.Reflection;

namespace GitPackage.Tests.App_Assets;

internal static class Assets
{
  internal static class DiffCheckoutTests
  {
    internal static IDirectoryInfo LatestChangesInBranch(IDirectoryInfo files)
    {
      var expectedItems = new[]
      {
        "readme.md", "item0.deleted.txt",
        "Folder2/item1.txt"
      };

      var resourceRoot = string.Join('.',
        typeof(DiffCheckoutTests).Namespace,
        nameof(DiffCheckoutTests), nameof(LatestChangesInBranch), 
        "expected");

      var root = files.GetFolder("App_Assets", nameof(DiffCheckoutTests), nameof(LatestChangesInBranch),
        "expected");

      var assembly = Assembly.GetExecutingAssembly();
      foreach (var item in expectedItems)
      { 
        using var rd = assembly.GetManifestResourceStream(
          $"{resourceRoot}.{item.Replace('/', '.')}") ??
          throw new Exception("Failed to find resource");

        var file = root.GetFile(item);
        using var wr = file.EnsureDirectory().Create();

        rd.CopyTo(wr);
      }

      return root;
    }
  }

  internal static class GlobCheckoutTests
  {
    internal static IDirectoryInfo CheckoutAll(IDirectoryInfo files)
    {
      var items = new[]
      {
        "readme.md",
        "Folder2/item1.txt",
        "Folder2/item2.txt",
      };

      var resourceRoot = string.Join('.',
        typeof(GlobCheckoutTests).Namespace, nameof(GlobCheckoutTests),
        nameof(CheckoutAll), "expected");

      LoadTree(resourceRoot, files, items);
      return files;
    }

    internal static IDirectoryInfo CheckoutFiltered(IDirectoryInfo files)
    {
      var items = new[]
      {
        "Folder2/item1.txt",
        "Folder2/item2.txt",
      };

      var resourceRoot = string.Join('.',
        typeof(GlobCheckoutTests).Namespace, nameof(GlobCheckoutTests),
        nameof(CheckoutFiltered), "expected");

      LoadTree(resourceRoot, files, items);
      return files;
    }
  }

  internal static Stream CoreUtilGitPackage()
    => Assembly.GetExecutingAssembly()
          .GetManifestResourceStream(typeof(Assets), "CoreUtil.gitpackage") ??
       throw new Exception("Resource not found");
  
  private static void LoadTree(string resourceNameRoot, IDirectoryInfo target, string[] items)
  {
    var assembly = Assembly.GetExecutingAssembly();

    foreach (var item in items)
    {
      var file = target.GetFile(item);
      using var wr = file.EnsureDirectory().Create();

      var rd = assembly.GetManifestResourceStream(
        $"{resourceNameRoot}.{item.Replace('/', '.')}")!;

      rd.CopyTo(wr);
    }
  }
}