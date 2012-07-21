using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.DebugUi.Infrastructure
{
    public interface ILogInterceptor
    {
        event EventHandler<LogInterceptedEventArgs> LogIntercepted;
    }

    public class LogInterceptedEventArgs : EventArgs
    {

        public LogMessage Message { get; private set; }

        public LogInterceptedEventArgs(LogMessage message)
        {
            Message = message;
        }
    }
}
