using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using Proximo.Cqrs.Core.Support;

namespace Sample.Server.Core.Initialization
{
    public class ReplayableDomainEventHandlerInitializer : IDomainEventHandlerInitializer
    {
        private IDomainEventHandlerCatalog _catalog;
        private IRawEventStore _rawEventStore;
        private ILogger _logger;

        public ReplayableDomainEventHandlerInitializer(
            IDomainEventHandlerCatalog catalog,
            IRawEventStore rawEventStore,
            ILogger logger)
        {
            _catalog = catalog;
            _rawEventStore = rawEventStore;
            _logger = logger;
        }

        public void Initialize(object domainEventHandler)
        {
            if (domainEventHandler is IReplayable)
            {
                _logger.SetOpType("replay", domainEventHandler.GetType().Name);
                IReplayable replayableHandler = (IReplayable)domainEventHandler;
                if (replayableHandler.ShouldReplay())
                {
                    //replay events
                    var invokerOfThisType = _catalog.GetAllHandlerForSpecificHandlertype(domainEventHandler.GetType());
                    var eventList = invokerOfThisType
                        .Select(i => i.Value.HandledType)
                        .ToArray();
                    _logger.Info("[replay] - Handler " + domainEventHandler.GetType().Name + " handle " + eventList.Length + " domain events\n" +
                            eventList.Select(t => t.Name).Aggregate((s1, s2) => s1 + "\n" + s2));
                    replayableHandler.StartReplay();
                    var allEvents = _rawEventStore.LoadEvents(eventList);
                    Int32 eventcount = 0;
                    foreach (var domainEvent in allEvents)
                    {
                        invokerOfThisType[domainEvent.GetType()].Invoke(domainEvent);
                        eventcount++;
                    }
                    replayableHandler.EndReplay();
                    _logger.Info("[replay] - replayed " + eventcount + " events for handler " + domainEventHandler.GetType().Name);
                }
                _logger.RemoveOpType();
            }
        }
    }
}
