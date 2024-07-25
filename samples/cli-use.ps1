# Demo to get k8s yaml for 
# a nfs based volume provider

# install as global tool.
dotnet tool uninstall git-get --global
dotnet tool install git-get --global

#check verions installed 
git-get about

$dir = "./CliDemo/nfs-provisioner"

New-Item -ItemType Directory -Path $dir -ErrorAction SilentlyContinue

# check whats in default cache
git-get info --target-path:$dir

#get some protos
$origin = "https://github.com/kubernetes-sigs/nfs-subdir-external-provisioner/"
$version = "tag/nfs-subdir-external-provisioner-4.0.17"
$filter = "*.yaml"

git get $dir --origin:$origin --version:$version --filter:$filter --force:all

#re-run, but now I want different version
git get $dir --version:branch/master

#nothing to do next time
git get $dir

#get updates from origin.
git get $dir --force:all

