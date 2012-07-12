using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Purchases.Domain.Events
{
    public class BillOfLadingCreated : IDomainEvent
    {
        public Guid Id { get; protected set; }
        public DateTime IssuedAt { get; protected set; }
        public string Number { get; protected set; }
        public DateTime ReceivedAt { get; protected set; }

        public BillOfLadingCreated(Guid id, DateTime issuedAt, string number, DateTime receivedAt)
        {
            Id = id;
            IssuedAt = issuedAt;
            Number = number;
            ReceivedAt = receivedAt;
        }
    }
}
