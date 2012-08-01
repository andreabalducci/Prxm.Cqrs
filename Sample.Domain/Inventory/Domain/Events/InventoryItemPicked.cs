using System;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemPicked : DomainEvent
    {
        public Guid AggregateId { get; protected set; }
        public decimal Quantity { get; protected set; }

        public InventoryItemPicked(Guid aggregateId, decimal quantity)
        {
            AggregateId = aggregateId;
            Quantity = quantity;
        }

        public InventoryItemPicked()
        {
        }

        public override string ToString()
        {
            return "Picked quantity " + Quantity + " from item " + AggregateId;
        }
    }
}