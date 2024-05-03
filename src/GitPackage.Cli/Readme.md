__GitPackage CLI__

A tool for re-using git repositories as source packages.

__Options__

Check version
> --version or -v

Main interface is via a JSON object:

> GitPackage inline-JSON

JSON can be provided using inpu file via 
> -f options
> GitPackage -f myinput.json

Can also provide multiple JSON's for processing via a input
[JSONL](https://jsonlines.org/) file 
> GitPackage -f myinput.jsonl


