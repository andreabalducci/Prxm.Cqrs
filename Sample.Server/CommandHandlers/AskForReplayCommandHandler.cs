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
using Sample.QueryModel.Storage.Readers;

namespace Sample.Server.CommandHandlers
{
	/// <summary>
	/// this is a first implementation: it has a side effect - if an event handler generates new command they lead to new committs and events being added to the eventstore which is wrong
	/// </summary>
	public class AskForReplayCommandHandler : ICommandHandler
	{
		public AskForReplayCommandHandler(ILogger logger, IStoreEvents eventStore, MongoDatabase db)
		{
			_logger = logger;
			_store = eventStore;
		    _db = db;
		}
		private ILogger _logger;
		private IStoreEvents _store;

        // ugly as hell: extend the modelwriter or wrap this
	    private MongoDatabase _db;

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

		public void AskForReplay(AskForReplayCommand command)
		{
			// ask the engine to perform a complete event replay
            _logger.Debug("Commits Replay Start");

            // ugly: let's drop the query model database for this test command
            _db.Drop();

			// get all the commits and related events
			var commitList = _store.Advanced.GetFrom(DateTime.MinValue);
            _logger.Debug(string.Format("Processing {0} commits", commitList.Count()));

			// first attempt use our original IDomainEventRouter to send the events to our eventhandlers

			foreach (var commit in commitList)
			{
				if (commit.Headers.Count > 0)
                    _logger.Debug(string.Format("Commit Header {0}", DumpDictionaryToString(commit.Headers)));

				foreach (var committedEvent in commit.Events)
				{
                    _logger.Debug(string.Format("Replaying event: {0}", committedEvent.Body.ToString()));
					
					if (committedEvent.Headers.Count > 0)
                        _logger.Debug(string.Format("Event Header {0}", DumpDictionaryToString(committedEvent.Headers)));

					// it has side effects, like generating new commits on the eventstore
					//OriginalDomainEventRouter.Dispatch(committedEvent.Body);
					SpecificDomainEventRouter.Dispatch(committedEvent.Body);

                    _logger.Debug("Event Replay Completed");
				}
			}

            _logger.Debug("Commits Replay Completed");
		}

		private string DumpDictionaryToString(IDictionary<string, object> data)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var e in data)
			{
				sb.AppendFormat("{0} - {1}", e.Key, e.Value.ToString());
			}
			return sb.ToString();
		}
	}
}
