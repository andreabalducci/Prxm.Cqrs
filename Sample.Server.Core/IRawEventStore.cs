using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Server.Support
{
    /// <summary>
    /// this interface permits to store events outside of EventStore library
    /// this is useful if we want to store event in a way that is more useful to 
    /// query for rebuilder, etc.
    /// </summary>
    public interface IRawEventStore
    {
        void SaveEvent(PersistedDomainEvent evt);

        IEnumerable<DomainEvent> LoadEvents(params Type[] eventsType);
    }
}
