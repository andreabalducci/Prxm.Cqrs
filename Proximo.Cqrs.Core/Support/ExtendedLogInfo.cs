using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Core.Support
{
	/// <summary>
	/// inherits from this class to provide properties for extended logging information
	/// 
	/// in log4net
	/// Each of the properties will be translated to a custom propery attached to the log entry,
	/// other log engines will use their own method
	/// 
	/// todo: move to a 'Logging' namespace
	/// </summary>
	public abstract class ExtendedLogInfo
	{
	}
}
