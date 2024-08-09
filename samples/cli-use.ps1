# Demo to get k8s yaml for 
# a nfs based volume provider

# (re) install as global tool.
dotnet tool uninstall git-get --global
dotnet tool install git-get --global --no-cache

#check version installed 
git-get about

$dir = "./CliDemo/nfs-provisioner"

# check whats in default cache
git-get info --target-path:$dir

#get some protos
$origin = "https://github.com/kubernetes-sigs/nfs-subdir-external-provisioner.git"
$version = "tag/nfs-subdir-external-provisioner-4.0.17"
$filter = "*.yaml"

git get $dir --origin:$origin --version:$version --filter:$filter --force:all

#re-run, but now I want different version
git get $dir --version:branch/master

#but all i want is the deploy yaml
git get $dir --get-root:/deploy

#nothing to do next time
git get $dir

#get updates from origin.
git get $dir --force:all

#add a work-tree to the cached repository
$workTreeDir = [IO.Path]::GetFullPath("$pwd/./CliDemo/WorkTree");
Remove-Item -Path $workTreeDir -Recurse -Force

$repoDir = "$(git get where --origin:$origin)"
pushd $repoDir
 git worktree prune
 git worktree add $workTreeDir master
popd

