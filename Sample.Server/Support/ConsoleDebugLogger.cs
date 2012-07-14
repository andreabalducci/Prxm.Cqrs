using System;
using Proximo.Cqrs.Core.Support;

namespace Sample.Server.Support
{
    public class ConsoleDebugLogger : IDebugLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }


        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}