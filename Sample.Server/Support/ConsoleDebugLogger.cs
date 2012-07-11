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
    }
}