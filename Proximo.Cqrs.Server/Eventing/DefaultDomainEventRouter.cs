using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    public class DefaultDomainEventRouter : IDomainEventRouter
    {
        private readonly IDomainEventHandlerCatalog _domainEventHandlerCatalog;

        public DefaultDomainEventRouter(IDomainEventHandlerCatalog domainEventHandlerCatalog)
        {
            _domainEventHandlerCatalog = domainEventHandlerCatalog;
        }

        public void Dispatch(Object @event)
        {
            var eventType = @event.GetType();
            var executorFunction = _domainEventHandlerCatalog.GetExecutorFor(eventType);
            executorFunction(@event as IDomainEvent);

            //var eventHandlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            
            //var handlers = _domainEventHandlerFactory.CreateHandlers(eventHandlerType);
            //if (handlers != null)
            //{
            //    foreach (var handler in handlers)
            //    {
            //        var handlerType = handler.GetType();
            //        MethodInfo mi = handlerType.GetMethod("Handle", new[] { eventType });
            //        mi.Invoke(handler, new[] { @event });
            //    }

            //    _domainEventHandlerFactory.ReleaseHandlers(handlers);
            //}
        }
    }
}
