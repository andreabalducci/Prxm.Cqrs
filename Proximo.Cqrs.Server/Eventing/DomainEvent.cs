using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using Proximo.Cqrs.Server.Aggregates;

namespace Proximo.Cqrs.Server.Eventing
{
    public abstract class DomainEvent : 
        IVersionedDomainEvent, 
        IPleaseVersionThisDomainEventAfterInternalStateChange
    {
        protected DomainEvent()
        {
        }

        public AggregateVersion Originator{ get; protected set; }

        public void TakeThis(AggregateVersion version)
        {
            Originator = version;
        }

      
    }
}
