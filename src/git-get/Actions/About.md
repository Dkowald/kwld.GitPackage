## dotnet gitget [Action] [Options]
 > A tool to get a set of files from a cloned repository
 > See [Source](https://github.com/Dkowald/kwld.GitPackage) for details  
 > Version: 99.0
-------
## Action
* about  - show this info
* init  - create / update a .gitget package status file
* info   - show info on cached repositories, and target .gitget if found
* where  - show local cache clone for specified origin
* target-path specify folder for .gitget file; defaults to current.
------
## Options
* --origin:[origin]  - source repository url
* --version:[version]  - source branch/[branch] or tag/[tag]
* --filter:[filter] - set of ',' seperated globs for target files defaults to all
* --cache:[cache]  - alternate local cache folder, defaults to HOME/.gitpackages
* --force:[force]  - force re-get even if already have a commit for [b]ranch, [t]ag or [a]ll
* --log-level:[LoggingLevel] - [t]race, [d]ebug, [i]nfo, [w]arn (default), [e]rror
--------
## e.g.
Get the readme and docs files for this project  
> dotnet git-get --origin:https://github.com/Dkowald/kwld.GitPackage.git --filter:/\*.md,doc/**/*