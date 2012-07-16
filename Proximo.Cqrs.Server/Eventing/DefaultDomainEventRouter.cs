using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Proximo.Cqrs.Core.Support;

namespace Proximo.Cqrs.Server.Eventing
{
    public class DefaultDomainEventRouter : IDomainEventRouter
    {
        private readonly IDomainEventHandlerCatalog _domainEventHandlerCatalog;
        private ILogger _logger;

        public DefaultDomainEventRouter(IDomainEventHandlerCatalog domainEventHandlerCatalog, ILogger logger)
        {
            _domainEventHandlerCatalog = domainEventHandlerCatalog;
            _logger = logger;
        }

        public void Dispatch(Object @event)
        {
            _logger.Info("[evt dispatcher] dispatching event " + @event.ToString());
            var eventType = @event.GetType();
            var handlerInvokerList = _domainEventHandlerCatalog.GetAllHandlerFor(eventType);
            _logger.Debug("[evt dispatcher] dispatching event " + @event.ToString() + " found " + handlerInvokerList.Count() + " handlers");
            foreach (var invoker in handlerInvokerList)
            {
                invoker(@event as IDomainEvent);
            }
            _logger.Debug("[evt dispatcher] dispatching event " + @event.ToString() + " done");
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
