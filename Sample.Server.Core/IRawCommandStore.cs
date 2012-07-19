using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Server.Core
{
    /// <summary>
    /// interface for a component that is capable of saving all commands 
    /// that were elaborated by the system.
    /// </summary>
    public interface IRawCommandStore
    {
        void Store(ExecutedCommand executedCommand);
    }
}
