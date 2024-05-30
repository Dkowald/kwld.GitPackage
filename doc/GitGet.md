# Overview

GitGet is a tool to sync a set of git repository files 
with a local folder.

It behaves like a git checkout, but the files are NOT in a git work-directory.

GitGet clones files to a local cache. 
Repositories are pulled with _--bare_ and stored in a (configurable) 
local cache folder.

The target folder includes a '.gitget' file to track options used, including resultant commit.
## Basic usage.

``` pwsh
#read .gitpackage in current directory, and get files if need.
dotnet git-get
```

Get latest readme from libgit2sharp
``` pwsh
# get readme.md from the libgit2sharp repo master branch
dotnet git-get --origin:https://github.com/libgit2/libgit2sharp.git --version:branch/master --filter:/readme.md
```

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

--version:[version-ref]

The git ref to use, such as _refs/heads/main_  
Can be a shorthand ref e.g 'head/main' or 'tag/v1.0'

-filter:{globs}  
A set of ';' delimited globs to select files. defaults to all.

glob entries are case-insensitive.

glob's are simple pattern match, using ** to match any-folder and * to match any char.


--cache:[cache-path] 

Alternate path for local cached repositories.

The cache is determined by:

- Use [cache-path] if provided
- Use _%HOME%/.gitpackages_ if %HOME% specified
- Use _%USERPROFILE%/.gitpackages_ (windows fallback)
- Use current-directory/.gitpackages as last resort.

--force:[force]

Force fetch and get.  
By default if the 'target-path/.gitpackage' has a commit, 
it is skipped when running git-get. 
This option forces a re-evaluate of the commit. 
If [version] is a branch ref, will also fetch latest from origin.

[force] is one of:

- branch : only force if [version-ref] is a branch ref
- tag : only force if [version-ref] is a tag ref
- all: force for both branch and tag references

--log-level:[log-level]

Logging reported whilst running.
[log-level] is one of

- c : Critical 
- e : Error
- w : Warning (default)
- i : Information
- d : Debug
- t : Trace

--target-path:[target-path]

Explicit define target path.  
Alternately used as the action.  
Defaults to current directory (./).

## .gitget format

The .gitget status file is a set of key-value pairs delimited by '/r/n'
Each line is of the form 'key=value'

|key|value|
|---|-----|
|origin| repository-origin|
|version| git ref to use|
|filter| glob filter(s) to use |
|commit| resolved git commit, set when files are extracted |

## .gitpackages

The .gitpackages folder maintains a local cache of cloned repositories.

Each repository is in a sub-folder made from the origin url.

IF a local git-repository is use, it is stored un .gitpackages/local/
