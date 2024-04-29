GitPackage aims to use Git Repositories in a similar way as NuGet packages.

## Get started

1. Add nuget GitPackage

``` xml
  <ItemGroup>
    <PackageReference Include="GitPackage" Version="99.0.0" />
  </ItemGroup>
```

2. Add git repository to project

``` xml
<ItemGroup>
  <GitPackage Include="FileInfoExtensions" Version="1.0.0" Uri="https://gist.github.com/df49cda62ea033be1eda60333921675a.git" />
</ItemGroup>
```

3. Build


