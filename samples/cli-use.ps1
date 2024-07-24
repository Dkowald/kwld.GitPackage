# install as global tool.
dotnet tool uninstall git-get --global
dotnet tool install git-get --global

#check verions installed 
git-get about

New-Item -ItemType Directory -Path CliDemo -ErrorAction SilentlyContinue

# check whats in default cache
git-get info --target-path:CliDemo

#get some protos
git get ./CliDemo --origin:https://github.com/Dkowald/kwld.CoreUtil.git --version:tag/v1.3.1 --filter:/*.md --force:all

#re-run, but now I want different version
git get ./CliDemo --version:branch/master

#nothing to do next time
git get ./CliDemo

#get updates from origin.
git get ./CliDemo --force:all

