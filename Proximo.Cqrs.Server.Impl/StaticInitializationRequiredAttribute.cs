using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Impl
{
    /// <summary>
    /// Identify a DomainHandler or a CommandExecutor that needs to be static inizialized.
    /// Static initialization means that I need to create one instance of the handler/command
    /// then call the Initialize Method 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StaticInitializationRequiredAttribute : Attribute
    {
    }
}
