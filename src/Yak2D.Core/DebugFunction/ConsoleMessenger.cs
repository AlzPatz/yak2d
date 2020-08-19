using System;

namespace Yak2D.Core
{
    public class ConsoleMessenger : IFrameworkMessenger
    {
        public ConsoleMessenger() { }

        public void Report(string message)
        {
            Console.WriteLine(message);
        }

        public void Shutdown()
        {
            Console.WriteLine("Yak2D Framework Messenger Shutting Down... Goodbye!");
        }
    }
}