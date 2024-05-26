# Overview

GitGet is a tool to sync a set of git repository files 
with a local folder.

It behaves like a git checkout, but the files are NOT in a git work-directory.

GitGet clones files to a local cache. 
Repositories are pulled with _--bare_ and stored in a (configurable) 
local cache folder.

## Basic usage.

Get latest readme from libgit2sharp
``` pwsh
dotnet git-get --origin:https://github.com/libgit2/libgit2sharp.git --version:branch/master --filter:/readme.md
```
This will clone libgit2sharp to a local cache folder, then 
get the latest '/readme.md' from master branch.

# Details

dotnet git-get [action] [options]

target-path is the default action

## action

> target-path

Gets files from the repository.  
Uses provided options and 'target-path/.gitpackages' for configuration.

If 'target-path/.gitpackages' already contains a commit,
no action performed (except with --force)

> init

Writes options to corresponding 'target-path/.gitpackage' 
without performing any actual git actions.

> info

Reports tool usage, and summary of current cached repositories.

If 'target-path/.gitpackage' also reports its details.

> where

Reports where the local cache clone is (will be) located.

## options

If no options provided, 'target-path/.gitpackage' file is read for details.
'target-path/.gitpackage' is updated to reflect other options (if provided).

--origin:[repository-origin]

The target repository uri

--verison:[version-ref]

The git ref to use, such as _refs/heads/main_  
Can be a shorthand ref e.g 'head/main' or 'tag/v1.0'

-filter:{globs}  
A set of ';' delimited globas to select files. defaults to all.

--cache:[cache-path]  

Alternate path for local cached repositories.

The cache is determined by:

- Use [cache-path] if provided
- Use _%HOME%/.gitpackages_ if %HOME% specified
- Use _%USERPROFILE%/.gitpackages_ (windows fallback)
- Use _target-path_/.gitpackages

--force:[force]

Force fetch and get.  
By default if the 'target-path/.gitpackage' has a commit, 
it is skiped when running git-get. 
This option forces a re-evaluate of the commit. 
If [version] is a branch ref, will also fetch latest from origin.

[force] is one of:

- branch : only force if [version-ref] is a branch ref
- tag : only force if [version-ref] is a tag ref
- all: force for both branch and tag references

--target-path:[target-path]

Explicit define target path.  
Also used as the action.  
Defaults to current directory (./).

## .gitpackage format

The gitpackage status file is a set of key-value pairs delimited by '/r/n'
Each line is of the form 'key=value'

|key|value|
|---|-----|
|origin| repository-origin|
|version| git ref to use|
|filter| glob filter(s) to use |
|commit| resolved git commit, set when files are extracted |
