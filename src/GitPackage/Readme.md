Tooling to include raw files from git repositories via CLI and MSBuild.

See [Docs](https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md) for more info.

#### GitPackage

MSBuild wrapper around git-get [src](https://github.com/Dkowald/kwld.GitPackage/tree/main)/[nuget](https://www.nuget.org/packages/git-get) to include git source files in a project

__e.g__  MSBuild to create a local clone and extract files from a repository.
``` xml
<PropertyGroup>
  <!--Alternate cli to use -->
  <GitPackage-Tool>dotnet tool git-get</GitPackage-Tool>

  <!--Alternate git repo cache-->
  <GitPackage-CachePath>c:/.gitpackage</GitPackage-CachePath>

  <!--More Logging-->
  <GitPackage-LogLevel>d</GitPackage-LogLevel>

  <!-- enable design-time git package restore -->
  <GitPackage-DesignTimeBuild>true</GitPackage-DesignTimeBuild>
</PropertyGroup>

<ItemGroup>
 <PackageReference Include="GitPackage" Version="99.0.0" />
</ItemGroup>

<ItemGroup>
  <!--Get doc files for this repo -->
  <GitPackage Include="Gitpackage/doc"
    Origin="https://github.com/Dkowald/kwld.GitPackage.git"
    Version = 'branch/main'
    Filter = '/*.md,doc/**/*.md'
    Ignore =  'release.md'
    GetRoot = 'doc/' >
    <!-- <User>username</User> -->
    <!-- <Password>password</Password> -->
  </GitPackage>
</ItemGroup>
```