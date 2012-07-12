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

namespace Sample.Server.CommandHandlers
{
	public class AskForReplayCommandHandler : ICommandHandler<AskForReplayCommand>
	{
		public AskForReplayCommandHandler(IDebugLogger logger, IStoreEvents eventStore)
		{
			_logger = logger;
			_store = eventStore;
		}
		private IDebugLogger _logger;
		private IStoreEvents _store;

		/// <summary>
		/// injected by castle windsor, it contains the very same event handles that are configured in the engine
		/// </summary>
		public IDomainEventRouter OriginalDomainEventRouter { get; set; }

		public void Handle(AskForReplayCommand command)
		{
			// ask the engine to perform a complete event replay
			_logger.Log("Commits Replay Start");

			// get all the commits and related events
			var commitList = _store.Advanced.GetFrom(DateTime.MinValue);
			_logger.Log(string.Format("Processing {0} commits", commitList.Count()));

			// first attempt use our original IDomainEventRouter to send the events to our eventhandlers

			foreach (var commit in commitList)
			{
				if (commit.Headers.Count > 0)
					_logger.Log(string.Format("Commit Header {0}", DumpDictionaryToString(commit.Headers)));

				foreach (var committedEvent in commit.Events)
				{
					_logger.Log(string.Format("Replaying event: {0}", committedEvent.Body.ToString()));
					
					if (committedEvent.Headers.Count > 0)
						_logger.Log(string.Format("Event Header {0}", DumpDictionaryToString(committedEvent.Headers)));

					OriginalDomainEventRouter.Dispatch(committedEvent.Body);

					_logger.Log("Event Replay Completed");
				}
			}

			_logger.Log("Commits Replay Completed");
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
