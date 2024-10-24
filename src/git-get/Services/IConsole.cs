namespace GitGet.Services;

public interface IConsole
{
    public TextWriter Out { get; }

    public TextWriter Error { get; }
}
