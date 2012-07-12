using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemReceived : IDomainEvent
    {
        public decimal Quantity { get; set; }
    }
}