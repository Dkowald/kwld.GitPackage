# Overview

GitGet is a simple tool to sync a set of git repository files 
with a local folder.

It behaves like a git checkout, but the files are NOT in a git work-directory.

Repositories are pulled with --bare and stored in a (configurable) 
local cache folder.


## Basic usage.

The easiest approa
Create a text file in the desired local file:

__Branch__  
The following directs gitget to fetch the latest 
commit for branch main in repository my/repo.git
```
Include = https://github.com/my/repo.git
Version = branch/main
Filter = **/*
```



