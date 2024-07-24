using GitGet.Services;

namespace GitPackage.Tests.TestHelpers
{
    public class FakeConsole : IConsole
    {
        private readonly StringWriter _out = new();
        private readonly StringWriter _error = new();

        public string StdOut
        {
            get{
                _out.Flush();
                return _out.GetStringBuilder().ToString();

            }
        }

        public string? StdError { get; private set; }

        public TextWriter Out => _out;

        public TextWriter Error => _error;
    }
}
