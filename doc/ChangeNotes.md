This is a re-boot of the now retired
[GitPackage](https://github.com/Dkowald/GitPackage)

This version splits the functionality into a CLI (git-get),
and a wrapping MSBuild item GitPackage.  

It also moves away from leveraging the git cli, 
replacing it with in-code git capabilities via 
[LibGit2Sharp](https://github.com/libgit2/libgit2sharp).

This should provide a more flexability on how to consume a git repository.

As a concequence, if you are using the current GitPackage,
you will need to re-start with this newer format.
