using Sample.Infrastructure.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemLoaded : IDomainEvent
    {
        public int Quantity { get; set; }
    }
}