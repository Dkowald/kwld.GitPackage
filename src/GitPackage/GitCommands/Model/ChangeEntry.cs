using LibGit2Sharp;

namespace GitPackage.GitCommands.Model;

internal record ChangeEntry(ChangeKind Change, string Path, GitObject? Item);