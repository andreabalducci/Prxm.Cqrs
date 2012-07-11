using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Infrastructure.Messaging
{
	public class DomainEventEnvelope :  IMessage
    {
        public IDomainEvent Event { get; set; }
    }
}
