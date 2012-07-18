using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EventStore;
using EventStore.Dispatcher;

namespace Sample.Server.Support
{
     class EventDispatcherInterceptor : IPipelineHook
    {
        private IRawEventStore _rawEventStore;

        public EventDispatcherInterceptor(IRawEventStore rawEventStore)
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
                evt.EventName = @event.Body.GetType().FullName;
                evt.DomainEvent = @event.Body;
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
