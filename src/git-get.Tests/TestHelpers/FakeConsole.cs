using GitGet.Services;

namespace GitGet.Tests.TestHelpers
{
    public class FakeConsole : IConsole
    {
        private readonly StringWriter _out = new();
        private readonly StringWriter _error = new();

        public string StdOut {
            get {
                _out.Flush();
                return _out.GetStringBuilder().ToString();

            }
        }

        public string StdError {
            get {
                _error.Flush();
                return _error.GetStringBuilder().ToString();
            }
        }

        public TextWriter Out => _out;

        public TextWriter Error => _error;
    }
}
