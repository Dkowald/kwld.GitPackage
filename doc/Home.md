GitPackage aims to use Git Repositories in a similar way as other 
packaging systems, such as NuGet.

Originally built as a set of MSBuild Tasks, its now re-booted as a 
dotnet tool '_GitGet_', simplifying the MSBuild Tasks to be a wrapper around the tool.

#### Details
- [GitGet](./GitGet.md)
- [GitPackage](./GitPAckage.md)

## Get started: GitGet

Install the dotnet tool 
``` pwsh
dotnet tool install gitget
```

Create a git package status file, this is a simple mutli line 
key-value data file, usually called _.gitpackage_.

|key|value|
|---|-----|
|Url| the repository url|
|||
``` pwsh

```


## Get started: GitPackage 

1. Add nuget GitPackage
> Add nuget GitPackage to include the build tools.
``` xml
  <ItemGroup>
    <PackageReference Include="GitPackage" Version="99.0.0" />
  </ItemGroup>
```

2. Add git repository to project
> Download the *.md files from the git repository, put them in the 
>  project ./Other folder
``` xml
<ItemGroup>
  <GitPackage Include="./Other" 
    Uri="https://github.com/Dkowald/kwld.GitPackage" 
    Version="tag/1.0.0"    
    Filter="*.md*"
  />
</ItemGroup>
```

3. Build

During build this will create _./Other/.gitpackage_ config file,
And then execute GitGet with it.

During build:
 - clone repository to local cache if need
 - fetch latest from origin if need
 - select commit from the version branch or tag.
 - extract files that match filter.


