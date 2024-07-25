__git-get__

A cli tool for re-using git repositories as source packages.

See [Repo](https://github.com/Dkowald/kwld.GitPackage/blob/wip/layout/doc/Home.md) for more info.

__e.g__

``` pwsh
#install
dotnet tool install git-get --global

#get docs for this repository
$origin = "https://github.com/Dkowald/kwld.GitPackage.git"
$version = "branch/main"
$filter = "/*.md,doc/**/*.md"

git-get ./GitPackage --origin:$origin --version:$version --filter:$filter

#re-fresh git package
git-get ./GitPackage --force:branch

#review local cached repositories
git-get info

```
