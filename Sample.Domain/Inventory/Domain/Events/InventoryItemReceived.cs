using System;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemReceived : IDomainEvent
    {
        public Guid AggregateId { get; protected set; }
        public decimal Quantity { get; protected set; }

        public InventoryItemReceived(Guid aggregateId, decimal quantity)
        {
            AggregateId = aggregateId;
            Quantity = quantity;
        }

        public InventoryItemReceived()
        {
        }
    }
}