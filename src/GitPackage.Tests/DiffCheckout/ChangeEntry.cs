using LibGit2Sharp;

namespace GitPackage.Tests.DiffCheckout;

public record ChangeEntry(ChangeKind Change, string Path, GitObject? Item);