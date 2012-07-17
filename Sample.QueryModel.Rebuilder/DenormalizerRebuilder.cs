using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Eventing;
using Sample.QueryModel.Builder;

namespace Sample.QueryModel.Rebuilder
{
	/// <summary>
	/// holds on some information that are relevant to decide if a projection need to be run again
	/// </summary>
	public class DenormalizerRebuilder
	{
		private DenormalizersHashes _previosHashes;

		private readonly IDenormalizerCatalog _catalog;
		private readonly IStoreEvents _eventStore;
		private readonly IDenormalizersHashesStore _denormalizersHashesStore;
		private readonly IHashcodeGenerator _hashcodeGenerator;
		private readonly ILogger _logger;
		private readonly IDomainEventHandlerCatalog _domainEventHandlerCatalog;

		public DenormalizerRebuilder(IDenormalizerCatalog catalog, IDenormalizersHashesStore hashesStore,
			IHashcodeGenerator hashcodeGenerator, IDomainEventHandlerCatalog domainEventCatalog, IStoreEvents eventStore, ILogger logger)
		{
			_catalog = catalog;
			_eventStore = eventStore;
			_hashcodeGenerator = hashcodeGenerator;
			_domainEventHandlerCatalog = domainEventCatalog;
			_denormalizersHashesStore = hashesStore;
			_logger = logger;
		}

		public void Rebuild()
		{
			// get the list of previous hashes
			_previosHashes = _denormalizersHashesStore.Load();

			// create an instance of 

			// get the new list of denormalizers from the catalog
			DenormalizersHashes newHashes = new DenormalizersHashes();
			List<DenormalizerToRebuild> denormalizersToRebuild = new List<DenormalizerToRebuild>();
			//  cycle through the list and compute the hashes for each denormalizer
			foreach (var denorm in _catalog.Denormalizers)
			{
				DenormalizerHash hash = new DenormalizerHash();
				hash.Name = denorm.ToString();
				hash.Hash = _hashcodeGenerator.Generate(denorm);
				hash.Timestamp = DateTime.Now;
				newHashes.Hashes.Add(hash);
				var ri = new DenormalizerToRebuild(hash, denorm);

				// check this list with the previous one to find if we need to rebuild the data
				DenormalizerHash prev = null;
				if (_previosHashes != null)
					prev = _previosHashes.Hashes.Where(p => p.Name == hash.Name).FirstOrDefault();

				// rebuild the data if the denormalizer was not present before or if it was changed
				ri.IsRebuildNeeded = prev == null || prev.Hash != ri.Hash;
				if (ri.IsRebuildNeeded)
					denormalizersToRebuild.Add(ri);
			}
			// rebuild the data only for the denormalizer that are changed
			if (denormalizersToRebuild.Count > 0)
			{
				// ask the engine to perform a complete event replay
				_logger.Info("Commits Replay Start");

				// get all the commits and related events
				var commitList = _eventStore.Advanced.GetFrom(DateTime.MinValue);
				_logger.Info(string.Format("Processing {0} commits", commitList.Count()));

				foreach (var commit in commitList)
				{
					if (commit.Headers.Count > 0)
						_logger.Info(string.Format("Commit Header {0}", DumpDictionaryToString(commit.Headers)));

					foreach (var committedEvent in commit.Events)
					{
						_logger.Info(string.Format("Replaying event: {0}", committedEvent.Body.ToString()));

						if (committedEvent.Headers.Count > 0)
							_logger.Info(string.Format("Event Header {0}", DumpDictionaryToString(committedEvent.Headers)));

						// it has side effects, like generating new commits on the eventstore
						//OriginalDomainEventRouter.Dispatch(committedEvent.Body);
						var eventType = committedEvent.Body.GetType();
						var handlerList = _domainEventHandlerCatalog.GetAllHandlerFor(eventType);
						foreach (var invoker in handlerList)
						{
							if (typeof(IDomainEventDenormalizer).IsAssignableFrom(invoker.DefiningType))
							{
								invoker.Invoke(committedEvent.Body as IDomainEvent);
							}
						}

						_logger.Info("Event Replay Completed");
					}
				}
				// persist the new series of hashes
				_denormalizersHashesStore.Save(newHashes);
			}
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

	public class DenormalizerToRebuild : DenormalizerHash
	{
		public Type DenormalyzerType { get; set; }

		public bool IsRebuildNeeded { get; set; }

		public DenormalizerToRebuild(DenormalizerHash hash, Type t)
		{
			this.Hash = hash.Hash;
			this.Name = hash.Name;
			this.Timestamp = hash.Timestamp;
			this.DenormalyzerType = t;
		}
	}


}
