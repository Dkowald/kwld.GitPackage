GitPackage aims to use Git Repositories in a similar way as other 
packaging systems.

Originally built as a set of MSBuild Tasks, its now re-booted as a 
dotnet tool '_git-get_', simplifying the MSBuild Tasks to be a wrapper around the tool.

#### Details
- [GitGet](./GitGet.md)
- [GitPackage](./GitPAckage.md)

## Get started: Git-Get

Install the dotnet tool 
``` pwsh
dotnet tool install git-get
```

Get the docs for another project.
``` pwsh
$origin = "https://github.com/Dkowald/kwld.CoreUtil.git"
$filter = "/docs/**/*"
$version = "branch/master"

mkdir docs
dotnet run git-get ./docs --origin:$origin --filter:$filter --version:$version
```

## Get started: GitPackage 

1. Add nuget GitPackage
> Add nuget GitPackage to include the build tools.
``` pwsh
dotnet add package GitPackage 
```

2. Add git repository to project
``` xml
<ItemGroup>
  <GitPackage 
    Include="./External/CoreUtil/" 
    Origin="https://github.com/Dkowald/kwld.CoreUtil.git" 
    Version="tag/1.3.2"
    Filter="/docs/**/*.md"
  />
</ItemGroup>
```

3. Build
```pwsh
#build will clone repo; and get files
dotnet build
cd ./External/CorUtil
```
