using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Proximo.Cqrs.Server.Commanding
{
    /// <summary>
    /// Represent the catalog that is able to discover and enlist 
    /// command that are available to the system.
    /// </summary>
    public interface ICommandHandlerCatalog
    {
        /// <summary>
        /// It accepts the type of the command you want to execute and gives 
        /// you bach a function that accepts the command to execute. It completely
        /// remove any decision for the Default Command Router.
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        CommandInvoker GetExecutorFor(Type commandType);
    }
}
