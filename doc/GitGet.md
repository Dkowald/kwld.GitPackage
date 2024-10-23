# Overview

git-get is a tool to sync a set of git repository files 
with a local folder.

It behaves like a git checkout, but the files are NOT in a git work-directory.

git-get clones repositories to a local cache. 
Repositories are pulled with _--bare_ and stored in a (configurable) 
local cache folder.

The target folder includes a '.gitget' file to track options used, including resultant commit.

**Note:**  
git submodules NOT supported.


## Basic usage.

``` pwsh
#install
dotnet tool install git-get --global

#read .gitget in current directory, and get files if need.
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

### target-path

Gets files from the repository.  
Uses provided options and 'target-path/.gitget' for configuration.

If 'target-path/.gitget' already contains a commit,
no action performed (except with --force)

If commit is missing, triggers the get flow:

- clone to local cache if needed.
- if [version] is branch, fetch latest from origin.
- if [version] is tag, and it's not found locally, fetch latest from origin.
- check can find commit for [version] in the repository (fail if not found)
- delete all files in target folder, replace with repository files.
- update .gitget to include the used commit.

### init

Writes options to corresponding 'target-path/.gitget' 
without performing any actual git actions.

overwrites existing package file with changes (if needed).

### about

Reports version info, and cli summary.

### info

Reports summary of current cached repositories.

Also uses other options, allong with 
'target-path/.gitget' (if found) 
to report.

### where

Reports where the local cache clone is (will be) located.

This can be used to perform regular git actions on a local cached 
repository

```pwsh
#checkout worktree for cached repository

$url = 'https://github.com/rsafier/DotNetGlob.git'

$repo = dotnet gitget where --origin:https://github.com/rsafier/DotNetGlob.git

pushd $repo
git worktree add ./

```

## options

Options are read from the coresponding arguments,
them merged with values found in _target_path/.gitget

This means you can provide a minimal cli call and reuse previous options 
saved in _target_path/.gitget_

### --origin:[repository-origin]

The target repository uri

### --version:[version-ref]

The short-hand git ref to use
 - branch/main - refer to main branch
 - tag/v1.0 - refer to tag v1.0

Alternatly, can be a explit git branch or tag ref
- refs/remotes/origin/main
- refs/tags/v1.0

Note: _Must_ be for origin remote when using explit branch ref.

### --filter:[globs]
A set of comma ',' delimited globs to select files.  
Defaults to all: **/*.  
glob entries are case-insensitive.  
glob's are simple pattern match, using ** to match any-folder and * to match any char.

### --ignore:[globs]
A set of comma ',' delimited globs to ignore some selected files.
Defaults to none: null


### --get-root:[get-root]
A sub-path withing the origin repository tree to 
collect files from. Only repository files withing this path will be 
extracted. 

This is useful with deep structured repositories, where all the required files 
reside in a particular sub-folder.

Defaults to root: '/'

### --cache:[cache-path]

Alternate path for local cached repositories.

The cache is determined by:

- Use [cache-path] if provided
- Use _%HOME%/.gitpackages_ if %HOME% specified
- Use _%USERPROFILE%/.gitpackages_ (windows fallback)
- Use current-directory/.gitpackages as last resort.

### --force:[force]

Force re-get of files.

By default if the 'target-path/.gitget' has a commit, 
it is skipped when running git-get. 

This option forces a re-evaluate of the commit.   
If [version] is a branch ref, will also force a fetch from origin.

[force] is one of:

- branch : only force if [version-ref] is a branch ref
- tag : only force if [version-ref] is a tag ref
- all: force for either a branch or tag references

### --log-level:[log-level]

Logging reported whilst running.
[log-level] is one of

- c : Critical 
- e : Error
- w : Warning (default)
- i : Information
- d : Debug
- t : Trace

### --target-path:[target-path]

Explicit define target path.  
Alternately used as the action.  
Defaults to current directory (./).

### --user:[username]
User name for private / secured Git access.

### --password:[password]
Pasword for private / secured Git access.

## .gitget format

The .gitget status file is a set of key-value pairs delimited by '/r/n'
It uses the same format style as [.env](https://dotenvx.com/docs/env-file#format)  
Each line is of the form 'key=value'

|key|value|
|---|-----|
|origin| repository-origin|
|version| git ref to use|
|filter| glob filter(s) to use |
|commit| resolved git commit, set when files are extracted. |

## .gitpackages

The .gitpackages folder maintains a local cache of cloned repositories.

Each repository is in a sub-folder made from the origin url.

If a local git-repository is use, it is stored under .gitpackages/local/
