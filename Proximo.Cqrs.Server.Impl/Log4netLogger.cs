using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Support;
using log4net;

namespace Proximo.Cqrs.Server.Impl
{
    public class Log4netLogger : ILogger
    {

        ILog Logger { get; set; }

        public bool IsDebugEnabled
        {
            get { return Logger.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return Logger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return Logger.IsFatalEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return Logger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return Logger.IsWarnEnabled; }
        }

        /// <summary
        /// Without any arguments the logger created has name "root"
        /// </summary>
        public Log4netLogger()
        {
            this.Logger = LogManager.GetLogger("root");
        }

        public Log4netLogger(Type containingType)
        {
            this.Logger = LogManager.GetLogger(containingType);
        }

        public Log4netLogger(String loggerName)
        {
            this.Logger = LogManager.GetLogger(loggerName);
        }

        public void Debug(string message)
        {
            this.Logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            this.Logger.Debug(message, exception);
        }

        public void Info(string message)
        {
            this.Logger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            this.Logger.Info(message, exception);
        }

        public void Warn(string message)
        {
            this.Logger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            this.Logger.Warn(message, exception);
        }

        public void Error(string message)
        {
            this.Logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            this.Logger.Error(message, exception);
        }

        public void Fatal(string message)
        {
            this.Logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            this.Logger.Fatal(message, exception);
        }

      
    }
}
