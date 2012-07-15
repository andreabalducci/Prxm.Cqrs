using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Core.Support
{
    public sealed class NullDebugLogger : IDebugLogger
    {
        public void Log(string message)
        {
        }

        public void Error(string message)
        {
        }
    }
}
