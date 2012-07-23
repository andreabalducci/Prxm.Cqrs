using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Core.Support
{
    public sealed class NullLogger : ILogger
    {
        private static NullLogger _instance;
        public static NullLogger Instance { get { return _instance; } }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }

        public void Debug(string message)
        {

        }

        public void Debug(string message, Exception exception)
        {

        }

        public void Error(string message)
        {

        }

        public void Error(string message, Exception exception)
        {

        }

        public void ErrorFormat(string format, params object[] args)
        {

        }

        public void Fatal(string message)
        {

        }

        public void Fatal(string message, Exception exception)
        {

        }

        public void Info(string message)
        {

        }

        public void Info(string message, Exception exception)
        {

        }

        public void Warn(string message)
        {

        }

        public void Warn(string message, Exception exception)
        {

        }


        public void SetInThreadContext(string propertyName, string propertyValue)
        {
           
        }

        public void RemoveFromThreadContext(string propertyName)
        {

        }


        public void SetOpType(string optype, string opTypeIdentification)
        {
           
        }

        public void RemoveOpType()
        {
           
        }
    }
}
