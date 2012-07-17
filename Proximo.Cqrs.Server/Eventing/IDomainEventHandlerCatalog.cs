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
        //IEnumerable<Action<IDomainEvent>> GetAllHandlerFor(Type domainEventType);
    }

    /// <summary>
    /// a class that holds some detailed information about the functions that are used to handle 
    /// </summary>
    public class DomainEventInvoker
    {
        // the actual function that have to be invoked in order to handle the event
        public Action<IDomainEvent> Invoker;

        /// <summary>
        /// the type in which the onvoker is defined
        /// </summary>
        public Type DefiningType;

        public DomainEventInvoker(Action<IDomainEvent> invoker, Type definingType)
        {
            Invoker = invoker;
            DefiningType = definingType;
        }

        public void Invoke(IDomainEvent @event)
        {
            Invoker.Invoke(@event);
        }
    }
}
