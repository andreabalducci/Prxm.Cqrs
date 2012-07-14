using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using Sample.QueryModel.Builder;
using System.Reflection;

namespace Sample.Server.CommandHandlers
{
	/// <summary>
	/// first ugly implementation to see if it works:
	/// marker interface to register a specific event handler router
	/// </summary>
	public interface IDomainEventRouterForQueryModelRebuild : IDomainEventRouter
	{

	}

	public class DomainEventRouterForQueryModelRebuild : IDomainEventRouterForQueryModelRebuild
	{
        private readonly IDomainEventHandlerCatalog _domainEventHandlerCatalog;

        public DomainEventRouterForQueryModelRebuild(IDomainEventHandlerCatalog domainEventHandlerCatalog)
        {
            _domainEventHandlerCatalog = domainEventHandlerCatalog;
        }

		public void Dispatch(object @event)
		{
            var eventType = @event.GetType();
            var executorFunction = _domainEventHandlerCatalog.GetExecutorFor(eventType);
            executorFunction(@event as IDomainEvent);
            //var eventType = @event.GetType();
            //var eventHandlerType = typeof(IDomainEventDenormalizer<>).MakeGenericType(eventType);

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
