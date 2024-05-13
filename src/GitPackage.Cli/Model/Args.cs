namespace GitPackage.Cli.Model;

internal class Args
{
    public Args(string[] args)
    {
        for (var idx = 0; idx < args.Length; idx++)
        {
            var arg = args[idx];

            ShowVersion = arg == "-v";

            if (arg == "-f")
            {
                if (++idx >= args.Length)
                    throw new Exception("Missing target file");

                DataFile = args[idx];
            }

            if (arg == "-c")
            {
                if (++idx >= args.Length)
                    throw new Exception("Missing cache path ");

                Cache = args[idx];
            }
        }
    }

    public bool ShowVersion { get; private set; } = false;

    public string? DataFile { get;}

    public string? Cache { get; }

}