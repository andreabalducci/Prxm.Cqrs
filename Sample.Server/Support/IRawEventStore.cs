using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Server.Support
{
    /// <summary>
    /// this interface permits to store events outside of EventStore library
    /// this is useful if we want to store event in a way that is more useful to 
    /// query for rebuilder, etc.
    /// </summary>
    interface IRawEventStore
    {
        void SaveEvent(PersistedDomainEvent evt);
    }
}
