namespace GitGet.Tests.TestHelpers;

public class CaptureConsole : IDisposable
{
    private readonly StringWriter _stdOut = new();
    private readonly StringWriter _stdError = new();

    public CaptureConsole()
    {
        Console.Out.Flush();
        Console.SetOut(_stdOut);
        Console.SetError(_stdError);
        IsRedirecting = true;
    }

    public bool IsRedirecting { get; private set; }

    public string? StdOut { get; private set; }
    public string? StdError { get; private set; }

    public CaptureConsole Flush()
    {
        _stdOut.Flush();
        StdOut = _stdOut.GetStringBuilder().ToString();

        _stdError.Flush();
        StdError = _stdError.GetStringBuilder().ToString();

        return this;
    }

    public void Dispose()
    {
        Flush();

        _stdOut.Dispose();
        _stdError.Dispose();

        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });

        IsRedirecting = false;

        GC.SuppressFinalize(this);
    }
}