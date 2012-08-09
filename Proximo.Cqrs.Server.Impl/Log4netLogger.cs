using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Support;
using log4net;
using log4net.Core;

namespace Proximo.Cqrs.Server.Impl
{
    public class Log4netLogger : Proximo.Cqrs.Core.Support.ILogger
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
        /// Without any arguments the logger created has name "log"
        /// </summary>
        public Log4netLogger()
        {
            this.Logger = LogManager.GetLogger("log");
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

		public void Debug(string message, Exception exception, ExtendedLogInfo info)
		{
			if (IsDebugEnabled)
			{
				LoggingEvent loggingEvent = new LoggingEvent(Logger.Logger.GetType(), Logger.Logger.Repository, Logger.Logger.Name, Level.Debug, message, exception);
				FillInLoggingEventWithExtendedProperties(info, loggingEvent);
				Logger.Logger.Log(loggingEvent);
			}
		}

        public void Info(string message)
        {
            this.Logger.Info(message);
        }

		public void Info(string message, Exception exception, ExtendedLogInfo info)
		{
			if (IsInfoEnabled)
			{
				LoggingEvent loggingEvent = new LoggingEvent(Logger.Logger.GetType(), Logger.Logger.Repository, Logger.Logger.Name, Level.Info, message, exception);
				FillInLoggingEventWithExtendedProperties(info, loggingEvent);
				Logger.Logger.Log(loggingEvent);
			}
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

		public void Warn(string message, Exception exception, ExtendedLogInfo info)
		{
			if (IsWarnEnabled)
			{
				LoggingEvent loggingEvent = new LoggingEvent(Logger.Logger.GetType(), Logger.Logger.Repository, Logger.Logger.Name, Level.Warn, message, exception);
				FillInLoggingEventWithExtendedProperties(info, loggingEvent);
				Logger.Logger.Log(loggingEvent);
			}
		}

        public void Error(string message)
        {
            this.Logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            this.Logger.Error(message, exception);
        }

		public void Error(string message, Exception exception, ExtendedLogInfo info)
		{
			if (IsErrorEnabled)
			{
				LoggingEvent loggingEvent = new LoggingEvent(Logger.Logger.GetType(), Logger.Logger.Repository, Logger.Logger.Name, Level.Error, message, exception);
				FillInLoggingEventWithExtendedProperties(info, loggingEvent);
				Logger.Logger.Log(loggingEvent);
			}
		}

        public void Fatal(string message)
        {
            this.Logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            this.Logger.Fatal(message, exception);
        }

		public void Fatal(string message, Exception exception, ExtendedLogInfo info)
		{
			if (IsFatalEnabled)
			{
				LoggingEvent loggingEvent = new LoggingEvent(Logger.Logger.GetType(), Logger.Logger.Repository, Logger.Logger.Name, Level.Fatal, message, exception);
				FillInLoggingEventWithExtendedProperties(info, loggingEvent);
				Logger.Logger.Log(loggingEvent);
			}
		}

		private static void FillInLoggingEventWithExtendedProperties(ExtendedLogInfo info, LoggingEvent loggingEvent)
		{
			// todo: add caching or fast reflection to improve performances a bit
			foreach (var pi in info.GetType().GetProperties())
			{
				loggingEvent.Properties[pi.Name] = pi.GetValue(info, null);
			}
		}

		public void SetInThreadContext(string propertyName, string propertyValue)
        {
            log4net.ThreadContext.Properties[propertyName] = propertyValue;
        }

        public void RemoveFromThreadContext(string propertyName)
        {
            log4net.ThreadContext.Properties.Remove(propertyName);
        }
		
        public void SetOpType(string optype, string opTypeIdentification)
        {
            log4net.ThreadContext.Properties["op_type"] = optype;
            log4net.ThreadContext.Properties["op_type_id"] = opTypeIdentification;
        }

		public void RemoveOpType()
        {
            log4net.ThreadContext.Properties.Remove("op_type");
            log4net.ThreadContext.Properties.Remove("op_type_id");
        }
    }
}
