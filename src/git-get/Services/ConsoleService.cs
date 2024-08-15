namespace GitGet.Services
{
    internal class ConsoleService : IConsole
    {
        public TextWriter Error => Console.Error;

        public TextWriter Out => Console.Out;
    }
}
