using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;
using Proximo.Cqrs.Server.Eventing;
using Sample.Server.Core;

namespace Sample.Server.Support
{
     class EventDispatcherToRawStoragePipelineHook : IPipelineHook
    {
        private IRawEventStore _rawEventStore;

        public EventDispatcherToRawStoragePipelineHook(IRawEventStore rawEventStore)
        {
            _rawEventStore = rawEventStore;
        }

        public void PostCommit(Commit committed)
        {
            foreach (var @event in committed.Events)
            {
                Debug.WriteLine(@event.Body.ToString());
                PersistedDomainEvent evt = new PersistedDomainEvent();
                evt.CommitSequence = committed.CommitSequence;
                evt.Timestamp = committed.CommitStamp;
                evt.EventType = @event.Body.GetType().FullName;
                evt.DomainEvent = (DomainEvent) @event.Body;
                evt.StreamId = committed.StreamId;
                evt.CommitId = committed.CommitId;
                _rawEventStore.SaveEvent(evt);
            }
        }

        public bool PreCommit(Commit attempt)
        {
            return true;
        }

        public Commit Select(Commit committed)
        {
            return committed;
        }

        public void Dispose()
        {

        }
    }
}
