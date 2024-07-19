# Overview

A wrapper around the git-get tool to include files from 
an external repository

## Task Items

### GitPackage
Includes files from an external git repository
- Identity: folder to contain the files
- Origin: git repository origin url.
- Verison: git ref to fetch from
- Filter: glob filter(s) for source files
- User: username for sprivate repo.
- Password: password for private repo

## Properties

- $(GitPackage-Tool)

Path to git-get.exe. 
Defaults to in-built version.

- $(GitPackage-CachePath)  
Optional to over-ride the default git-get cache.

- $(GitPackage-DesignTimeBuild)  
bool toggle to enable restore git packages as part of design time build.  
false by default.

- $(GitPackage-LogLevel)  
override the default log level.

## Build Targets

### gpRestore

Process __GetPackage__ items, getting files for those that
dont have a commit in the .gitpackage file.

### gpDesignTimeRestore
Design time build target.  
Runs gpRestore as part of Visual studio design builds.

