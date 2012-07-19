using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Server.Core.Initialization
{
    /// <summary>
    /// supports the concepts of an initializer of a domain event handler. This should
    /// be moved inside the core and used by the catalog to inizialize everything.
    /// </summary>
    public interface IDomainEventHandlerInitializer
    {
        void Initialize(Object domainEventHandler);
    }
}
