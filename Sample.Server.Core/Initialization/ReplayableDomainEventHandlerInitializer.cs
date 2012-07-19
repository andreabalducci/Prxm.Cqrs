using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Server.Core.Initialization
{
    public class ReplayableDomainEventHandlerInitializer : IDomainEventHandlerInitializer
    {
        private IDomainEventHandlerCatalog _catalog;
        private IRawEventStore _rawEventStore;

        public ReplayableDomainEventHandlerInitializer(
            IDomainEventHandlerCatalog catalog,
            IRawEventStore rawEventStore)
        {
            _catalog = catalog;
            _rawEventStore = rawEventStore;
        }

        public void Initialize(object domainEventHandler)
        {
            if (domainEventHandler is IReplayable)
            {
                IReplayable replayableHandler = (IReplayable)domainEventHandler;
                if (replayableHandler.ShouldReplay())
                {
                    //replay events
                    var invokerOfThisType = _catalog.GetAllHandlerForSpecificHandlertype(domainEventHandler.GetType());
                    var eventList = invokerOfThisType
                        .Select(i => i.Value.HandledType)
                        .ToArray();
                    replayableHandler.StartReplay();
                    var allEvents = _rawEventStore.LoadEvents(eventList);
                    foreach (var domainEvent in allEvents)
                    {
                        invokerOfThisType[domainEvent.GetType()].Invoke(domainEvent);
                    }
                    replayableHandler.EndReplay();
                }
            }
        }
    }
}
