Tooling to include raw files from git repositories via CLI and MSBuild.

See [Docs](https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md) for more info.

#### GitPackage

MSBuild wrapper around git-get to include git source files in a project

__e.g__  MSBuild to create a local clone and extract files from a repository.
``` xml
<ItemGroup>
 <PackageReference Include="GitPackage" Version="99.0.0" />
</ItemGroup>

<ItemGroup>
  <!--Get doc files for this repo -->
  <GitPackage Include="Gitpackage/"
    Origin="https://github.com/Dkowald/kwld.GitPackage.git"
    Version = 'branch/main'
    Filter = '/*.md,doc/**/*.md'>
  </GitPackage>
</ItemGroup>
```