using Microsoft.Extensions.Logging;

namespace GitPackage.Cli.Model;

internal class Config
{
    private readonly List<string> _errors = new();

    public static readonly string HomeUrl = "https://github.com/Dkowald/kwld.GitPackage";

    public static readonly string HomeRepositoryUrl = "https://github.com/Dkowald/kwld.GitPackage.git";

    public Config Read(string[] args)
    {
        _errors.Clear();
        var logLevel = "w";

        for (var idx = 0; idx < args.Length; idx++)
        {
            var key = args[idx];

            if (key == "-v")
            {
                ShowVersion = true;
                continue;
            }

            //all others are key-value
            if (++idx >= args.Length)
            {
                _errors.Add($"Argument {key} missing value");
                continue;
            }

            var value = args[idx];

            if (key == "-f") { DataFile = value; }
            else if (key == "-c") { Cache = value; }
            else if (key == "-l") { logLevel = value; }
            else { _errors.Add($"Argument {key} unknown"); break; }
        }

        LogLevel = ResolveLogLevel(logLevel);

        if (args.Length == 0)
            ShowVersion = true;

        return this;
    }

    public IReadOnlyList<string> Errors => _errors;

    public bool ShowVersion { get; private set; }

    public string? DataFile { get; private set; }

    public string? Cache { get; private set; }

    public LogLevel LogLevel { get; private set; }

    private LogLevel ResolveLogLevel(string value)
    {
        var lvl = value;

        if (lvl.Same("c"))
            return LogLevel.Critical;

        if (lvl.Same("e"))
            return LogLevel.Error;

        if (lvl.Same("w"))
            return LogLevel.Warning;

        if (lvl.Same("i"))
            return LogLevel.Information;

        if (lvl.Same("d"))
            return LogLevel.Debug;

        if (lvl.Same("t"))
            return LogLevel.Trace;

        _errors.Add($"Unknown LogLevel '{lvl}'");
        return LogLevel.Warning;
    }
}