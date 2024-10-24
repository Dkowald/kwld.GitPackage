Tooling to include raw files from git repositories via CLI and MSBuild.

See [Docs](https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md) for more info.

#### GitPackage

MSBuild wrapper around git-get [src](https://github.com/Dkowald/kwld.GitPackage/tree/main)/[nuget](https://www.nuget.org/packages/git-get) to include git source files in a project

Basic usage
``` xml
<ItemGroup>
 <PackageReference Include="GitPackage" Version="99.0.0" />
</ItemGroup>

<ItemGroup>
  <!--Get doc files for this repo -->
  <GitPackage Include="Gitpackage/doc"
    Origin  = "https://github.com/Dkowald/kwld.GitPackage.git"
    Version = 'branch/main'
    Filter  = '/*.md,doc/**/*.md'
    Ignore  =  'release.md'
    GetRoot = 'doc/' >
    <!--specify this version via git tag-->
    <!-- <Version>tag/v99</Version> -->

    <!--Credentials to read secure repo-->
    <!-- <User>username</User> -->
    <!-- <Password>password</Password> -->
  </GitPackage>
</ItemGroup>
```

Aditional options

``` xml
<PropertyGroup>
  <!--when re-build project; get latest from server if using a branch version--> 
  <!--true by default-->
  <GitPackage-UpdateOnRebuild>true</GitPackage-UpdateOnRebuild>

  <!--Specify an alternate cli to use -->
  <GitPackage-Tool>dotnet tool git-get</GitPackage-Tool>

  <!--Change the repo cach from ~/.gitpackages-->
  <GitPackage-CachePath>c:/.gitpackage</GitPackage-CachePath>

  <!--More Logging-->
  <!- (c)ritical (e)rror (w)arn (i)nfo (d)ebug (t)race ->
  <GitPackage-LogLevel>d</GitPackage-LogLevel>

  <!--Enable design-time git package restore -->
  <GitPackage-DesignTimeBuild>true</GitPackage-DesignTimeBuild>
</PropertyGroup>
</ItemGroup>
```