
Migration notes:

This is a re-boot of the earlier [GitPackage](https://github.com/Dkowald/GitPackage)

This time around, I want to remove the dependency on git cli;
and replace it with in-code git capabilities via LibGit2Sharp

This should provide a more flexability on how to consume a git repository.

But, as a concequence of leveraging additional libraries;
this now seperated the engin into its own packaged exe: GitGet
This avoids challenged with custom MSBuild task's and dependencies

see: 
https://natemcmaster.com/blog/2017/11/11/msbuild-task-with-dependencies/  
