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
        IEnumerable<DomainEventInvoker> GetAllHandlerFor(Type domainEventType);

        /// <summary>
        /// Retrieve all enumerated handlers type on the system.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetAllHandlers();

        /// <summary>
        /// We need the ability to replay all handler for a specific handler, thus the catalog
        /// should be able to give you a dictionary that stores all event handlers for a given
        /// handler type.
        /// </summary>
        /// <param name="handlerType">The type of the Handler that defines handlers</param>
        /// <returns>A dictionary where the key contains the type of domain event handled and the value
        /// is the Action that actually handles the event</returns>
        IDictionary<Type, DomainEventInvoker> GetAllHandlerForSpecificHandlertype(Type handlerType);


    }
}
