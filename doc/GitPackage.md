# Overview

A wrapper around the git-get tool to include files from 
an external repository

## Task Items

### GitPackage
Includes files from an external git repository  
__Item metadata__

|Name| Description |
|----|-------------|
|Identity|folder to contain the files|
|Origin|git repository origin url|
|Verison|git ref to fetch from|
|Filter|glob filter(s) for source files|
|Ignore|glob filter(s) to ignore selected files|
|GetRoot|Optional sub-path with repository to get from|
|User|username for private repo|
|Password|password for private repo| 

## Properties

- $(GitPackage-Tool)  
Path to git-get.exe. 
Defaults to in-built version.

- $(GitPackage-CachePath)  
Optional to over-ride the default git-get cache.

- $(GitPackage-DesignTimeBuild)  
Set to 'true' to enable restore git packages as part of design time build.  
false by default.

- $(GitPackage-LogLevel)  
override the default log level.

## Build Targets

### GPUpdate
Refresh all GitPackage items that use a branch

```pwsh
dotnet msbuild -t:GPUpdate
```

---

### gpRestore

Process __GitPackage__ items, getting files for those that
dont have a commit in the .gitget file.

### gpDesignTimeRestore
Design time build target.  
Runs gpRestore as part of Visual studio design builds.

