using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    public interface IDomainEventHandlerCatalog
    {
        /// <summary>
        /// Gets the list of invoker that are associated to all executors
        /// that are able to intercept that domain event.
        /// </summary>
        /// <param name="domainEventType"></param>
        /// <returns></returns>
        IEnumerable<Action<IDomainEvent>> GetAllHandlerFor(Type domainEventType);
    }
}
