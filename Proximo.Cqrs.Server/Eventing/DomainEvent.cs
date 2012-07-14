using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Aggregates;

namespace Proximo.Cqrs.Server.Eventing
{
    public abstract class DomainEvent : IDomainEvent
    {
        public VersionedAggregateId Source { get; protected set; }

        
    }
}
