# adds current debug build to path.
$path = "$PSScriptRoot/../src/git-get/bin/Debug/net8.0"
$path = Resolve-Path $path

if(! $env:Path.StartsWith("$path"))
{
  $env:Path = "$($path);$($env:Path)"
}
