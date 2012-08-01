using System;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemStocked : DomainEvent
    {
        public Guid AggregateId { get; protected set; }
        public decimal Quantity { get; protected set; }

        public InventoryItemStocked(Guid aggregateId, decimal quantity)
        {
            AggregateId = aggregateId;
            Quantity = quantity;
        }

        public InventoryItemStocked()
        {
        }

        public override string ToString()
        {
            return "Stocked quantity " + Quantity + " for item  " + AggregateId;
        }
    }
}