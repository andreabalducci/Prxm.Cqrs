using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Commands.System;
using Proximo.Cqrs.Server.Commanding;
using Proximo.Cqrs.Core.Support;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Persistence;
using EventStore.Serialization;
using Proximo.Cqrs.Server.Eventing;
using MongoDB.Driver;

namespace Sample.Server.CommandHandlers
{
	/// <summary>
	/// OBSOLETE: THIS IS A REAL FIRST TRY, it is not so smart blocking command handling to request a complete 
    /// replay of an handler, it can stale the command queue for too much time.
	/// </summary>
	public class AskForReplayHandlerReplayCommandHandler : ICommandHandler
	{
        public AskForReplayHandlerReplayCommandHandler(ILogger logger, IStoreEvents eventStore, IDomainEventHandlerCatalog domainEventHandlerCatalog)
		{
			_logger = logger;
			_store = eventStore;
            _domainEventHandlerCatalog = domainEventHandlerCatalog;
		}
		private ILogger _logger;
		private IStoreEvents _store;
        private IDomainEventHandlerCatalog _domainEventHandlerCatalog;

		/// <summary>
		/// injected by castle windsor, it contains the very same event handles that are configured in the engine
		/// it has a nasty side effect. even event handlers that leads to bussiness logic code are executed
		/// we just need to route the events only to the QueryModel eventhandlers
		/// </summary>
		public IDomainEventRouter OriginalDomainEventRouter { get; set; }

		/// <summary>
		/// injected by castle, it's a bit ugly and to be refactored at a later time.
		/// it's just to test if this strategy works
		/// </summary>
		public IDomainEventRouterForQueryModelRebuild SpecificDomainEventRouter { get; set; }

		public void AskForReplay(AskForSpecificHandlerReplayCommand command)
		{
			// ask the engine to perform a complete event replay
            _logger.Debug("Replay events for type " + command.Handlertype);

			// get all the commits and related events
			var commitList = _store.Advanced.GetFrom(DateTime.MinValue);
            _logger.Debug(string.Format("Processing {0} commits", commitList.Count()));

            Type handlerType = Type.GetType(command.Handlertype);
            var allHandlers = _domainEventHandlerCatalog.GetAllHandlerForSpecificHandlertype(handlerType);

			// first attempt use our original IDomainEventRouter to send the events to our eventhandlers
            foreach (var commit in commitList)
            {
                foreach (var committedEvent in commit.Events)
                {
                    if (!(committedEvent.Body is DomainEvent)) continue; //it is not a domain event.. probably there is some error.
                    DomainEvent evt = committedEvent.Body as DomainEvent;
                    if (allHandlers.ContainsKey(evt.GetType())) {

                        allHandlers[evt.GetType()].Invoke(evt);
                    }
                }
            }

            _logger.Debug("Commits Replay Completed");
		}


	}
}
