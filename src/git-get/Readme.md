__git-get__

A cli tool for re-using git repositories as source packages.

See [Docs](https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md) for more info.

See GitPackage [src](https://github.com/Dkowald/kwld.GitPackage/tree/main)/[nuget](https://www.nuget.org/packages/GitPackage)
 for usage in msbuild projects.   
__e.g__

``` pwsh
#install
dotnet tool install git-get --global --version 99.0.0

#get docs for this repository
$origin = "https://github.com/Dkowald/kwld.GitPackage.git"
$version = "branch/main"
$filter = "/*.md,doc/**/*.md"

git-get ./ --origin:$origin --version:$version --filter:$filter

#update to latest for branch
git-get ./ --force:all

#swap to a tagged version
git-get ./GitPackage --version:tag:v0.1

#review local cached repositories
git-get info

```
