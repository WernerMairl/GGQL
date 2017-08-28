using Microsoft.Extensions.Logging.Console.Internal;

namespace GGQL.Application
{
    internal class AnsiSystemConsole : IAnsiSystemConsole
    {
        public void Write(string message)
        {
            System.Console.Write(message);
        }

        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
