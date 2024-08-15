Tooling to include raw files from git repositories via CLI and MSBuild.

See [Docs](https://github.com/Dkowald/kwld.GitPackage/blob/wip/layout/doc/Home.md) for more info.

#### GitGet

CLI tool to create a local clone and extract files from a repository.

``` pwsh
# Install cli tool
dotnet tool install git-get

get the docs for this repo.
$origin = 
#write gitpackage file
echo `
Url=https://github.com/Dkowald/kwld.GitPackage `
Version=branch/main `
Filter=doc/**/*.md `
> .gitpackage

#clone and extract files in curent folder.
dotnet gitget
```

__Url__  

Specifies the target repository.   
This repository is cloned locally to a cache %HOME%/.gitpackage

__Version__ 

Specifies the particular commit to use; can be '_branch/_'or '_tag/_' 

__Filter__  
Glob patter to limit files. Can be multiple globs seperated by ';'

----

#### GitPackage

MSBuild wrapper around GitGet to include git source files in a project

__E.g__  MSBuild to create a local clone and extract files from a repository.
``` xml
<ItemGroup>
 <!--Include package-->
 <PackageReference Include="GitPackage" Version="99.0.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; buildtransitive</IncludeAssets>
 </PackageReference>
</ItemGroup>

<ItemGroup>
  <!--Get doc files for this repo -->
  <GitPackage Include="./external/kwld.GitPackage/"
    Url='https://github.com/Dkowald/kwld.GitPackage.git'
    Version = 'branch/main'
    Filter = 'doc/**/*.md'>
  </GitPackage>
</ItemGroup>
```
