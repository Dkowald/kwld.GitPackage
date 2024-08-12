namespace GitGet.Tests.TestHelpers;

public class EnvironmentVar : IDisposable
{
    private readonly string _key;
    private readonly string? _original;

    public EnvironmentVar(string key, string? value)
    {
        _key = key;
        _original = Environment.GetEnvironmentVariable(key);
        Environment.SetEnvironmentVariable(key, value);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(_key, _original);
        GC.SuppressFinalize(this);
    }
}