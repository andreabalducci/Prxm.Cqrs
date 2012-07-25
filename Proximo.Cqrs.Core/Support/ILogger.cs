using System;
namespace Proximo.Cqrs.Core.Support
{
	/// <summary>
	/// This is the interface for a generic logger.
	/// 
	/// todo: move to a 'Logging' namespace
	/// </summary>
	public interface ILogger
	{
		bool IsDebugEnabled { get; }

		bool IsErrorEnabled { get; }

		bool IsFatalEnabled { get; }

		bool IsInfoEnabled { get; }

		bool IsWarnEnabled { get; }

		void Debug(string message);

		void Debug(string message, Exception exception);

		void Debug(string message, Exception exception, ExtendedLogInfo info);

		void Error(string message);

		void Error(string message, Exception exception);

		void Error(string message, Exception exception, ExtendedLogInfo info);

		void Fatal(string message);

		void Fatal(string message, Exception exception);

		void Fatal(string message, Exception exception, ExtendedLogInfo info);

		void Info(string message);

		void Info(string message, Exception exception);

		void Info(string message, Exception exception, ExtendedLogInfo info);

		void Warn(string message);

		void Warn(string message, Exception exception);

		void Warn(string message, Exception exception, ExtendedLogInfo info);
		
		#region Contextual properties

		void SetInThreadContext(String propertyName, String propertyValue);

		void RemoveFromThreadContext(String propertyName);

		/// <summary>
		/// todo: refactor this method and create a new one that accept a class
		/// those properties will be added to the thread context
		/// </summary>
		/// <param name="optype"></param>
		/// <param name="opTypeIdentification"></param>
		void SetOpType(String optype, String opTypeIdentification);

		void RemoveOpType();

		#endregion
	}
}
