using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Proximo.Cqrs.Server.Eventing
{
    public class DefaultDomainEventRouter : IDomainEventRouter
    {
        private readonly IDomainEventHandlerFactory _domainEventHandlerFactory;

        public DefaultDomainEventRouter(IDomainEventHandlerFactory domainEventHandlerFactory)
        {
            _domainEventHandlerFactory = domainEventHandlerFactory;
        }

        public void Dispatch(object @event)
        {
            var eventType = @event.GetType();
            var eventHandlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            
            var handler = _domainEventHandlerFactory.CreateHandler(eventHandlerType);
            if (handler != null)
            {
                MethodInfo mi = eventHandlerType.GetMethod("Handle", new[] { eventType });
                mi.Invoke(handler, new[] { @event });

                _domainEventHandlerFactory.ReleaseHandler(handler);
            }
        }
    }
}
