# add tool to the path 
../install.ps1

Write-Host "About"
git get about

# get master readme doc.
$repo = "https://github.com/Dkowald/kwld.CoreUtil.git"
$ver = "branch/master"
git get ./demo1 --origin:$repo --version:$ver --log-level:d --filter:/*.md

pushd ./demo1

# re-run does nothing, already collected files.
git get  --log-level:i

# check the readme on older version
$ver = "tag/v1.3.1"
git get --version:$ver --log-level:d

popd

