# Overview

A wrapper around the git-get tool to include files from 
an external repository

## Items

### GitReference

- Identity: path to .gitpackage file
- Origin: git repository origin url.
- Verison: git ref to fetch from
- Filter: glob filter(s) for source files

## Targets

### GitPackRestore

Process __GetReference__ items, getting files for those that
dont have a commit in the .gitpackage file.

### GitPackUpdate

For any GitReference using a branch as a version.
Fetch latest for the repository, and update the files (if needed).

## Properties

> $(GitGet-CachePath)

Optional to over-ride the default git-get cache.

> $(GitPack-AutoRestore)

When true __GitPackRestore__ is used in design time build.

So, changes to .gitpackage file(s) are automaticy re-synced
