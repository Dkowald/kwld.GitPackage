param(

    [switch]$rebuild = $false
)
$TestRepository = "./App_Data/TestRepository"

Push-Location $PSScriptRoot
try{
    if(Test-Path $TestRepository -PathType Container)
    {
        if($rebuild){
            Write-Host "Rebuilding test repository"
            Remove-Item $TestRepository -Recurse -Force
        }else{
            write-host "Repository already exists"
            return;
        }
    }
    New-Item $TestRepository -ItemType Directory
    Push-Location $TestRepository
    try{
      git init
      New-Item "readme.md" -ItemType File -Value "readme.md" -Force
      New-Item "item0.txt" -ItemType File -Value "item0.txt" -Force
      New-Item "Folder1/item1.txt" -ItemType File -Value "item1.txt" -Force
      New-Item "Folder2/item2.txt" -ItemType File -Value "item2.txt" -Force

      git add -A
      git commit -am "init"
      git tag v0

      Set-Content "readme.md" -Value "Updated"
      Remove-Item ".\item0.txt" -Force
      Move-Item "Folder1/item1.txt" -Destination "folder2/item1.txt"
      git add -A
      git commit -am "update-del-move"
      git tag -a CheckoutAll -m "CheckoutAll"

    }finally{
        Pop-Location
    }
    

}
finally {
    Pop-Location
}
