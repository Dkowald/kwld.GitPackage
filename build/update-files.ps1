param(
    [string]$PackageVersion
)

push-location $PSScriptRoot

if([string]::IsNullOrWhiteSpace($PackageVersion)){
  write-host "PackageVersion not set, no work to do"
  return;
} else{
  write-host "Updating files to use version: $PackageVersion"
}

$tagName = "v$($PackageVersion)"

$GitGetDir = "../src/git-get"
$GitPackageDir = "../src/GitPackage"

$gitget_github = "https://github.com/Dkowald/kwld.GitPackage/blob/v$($PackageVersion)"
$gitget_nuget = "https://www.nuget.org/packages/git-get/"
$gitpackage_nuget = "https://www.nuget.org/packages/GitPackage"

$file = "$($GitGetDir)/Const.cs"
write-host "Update $file"
if(!(test-path -path $file)){throw "Cannot find target file: $file";}
$content = (Get-Content $file).replace(
    "HomeUrl = ""https://github.com/Dkowald/kwld.GitPackage""", 
    "HomeUrl = ""https://github.com/Dkowald/kwld.GitPackage/tree/$tagName""");
Set-Content -path $file -Value $content

$file = "$($GitGetDir)/Readme.md"
write-host "Update $file"
$content = (Get-Content $file)
$content = $content.replace("https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md", "https://github.com/Dkowald/kwld.GitPackage/blob/$($tagName)/doc/Home.md");
set-content -path $file -Value $content

$file = "$($GitPackageDir)/Readme.md"
write-host "Update $file"
$content = (Get-Content $file)
$content = $content.replace("https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md", "https://github.com/Dkowald/kwld.GitPackage/blob/$($tagName)/doc/Home.md");
$content = $content.replace("99.0.0", "$($PackageVersion)");
set-content -path $file -Value $content

pop-location