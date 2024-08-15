param(
    [string]$PackageVersion
)

push-location $PSScriptRoot

if([string]::IsNullOrWhiteSpace($PackageVersion)){throw "Version not set"}

$tagName = "v$($PackageVersion)"

$GitGetDir = "../src/git-get"
$GitPackageDir = "../src/GitPackage"

$gitget_github = "https://github.com/Dkowald/kwld.GitPackage/blob/v$($PackageVersion)"
$gitget_nuget = "https://www.nuget.org/packages/git-get/"
$gitpackage_nuget = "https://www.nuget.org/packages/GitPackage"

$file = "$($GitGetDir)/Const.cs"
if(!(test-path -path $file)){throw "Cannot find target file: $file";}
$content = (Get-Content $file).replace(
    "HomeUrl = ""https://github.com/Dkowald/kwld.GitPackage""", 
    "HomeUrl = ""https://github.com/Dkowald/kwld.GitPackage/tree/$tagName""");
Set-Content -path $file -Value $content

$file = "$($GitGetDir)/Readme.md"
$content = (Get-Content $file)
$content = $content.replace("https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md", "https://github.com/Dkowald/kwld.GitPackage/blob/$($tagName)/doc/Home.md");
set-content -path $file -Value $content

$file = "$($GitPackageDir)/Readme.md"
$content = (Get-Content $file)
$content = $content.replace("https://github.com/Dkowald/kwld.GitPackage/blob/main/doc/Home.md", "https://github.com/Dkowald/kwld.GitPackage/blob/$($tagName)/doc/Home.md");
$content = $content.replace("99.0.0", "$($PackageVersion)");
set-content -path $file -Value $content

#$src = "(docs/Home.md)"
#$target = "(" + [io.path]::Combine("https://github.com/Dkowald/kwld.CoreUtil/blob/", $tagName, "Readme.md") + ")"
#$content = $content.replace($src, $target)

#$src= "(https://github.com/Dkowald/kwld.CoreUtil)"
#$target = "(" + [io.path]::Combine("https://github.com/Dkowald/kwld.CoreUtil/blob/", $tagName) + ")"
#$content = $content.replace($src, $target)

#$src= "(https://www.nuget.org/packages/kwld.CoreUtil/)"
#$target = "(" + [io.path]::Combine("https://www.nuget.org/packages/kwld.CoreUtil/", $PackageVersion) + ")"
#$content = $content.replace($src, $target)

#Set-Content -path $file -Value $content

pop-location