using Sample.Infrastructure.Eventing;

namespace Sample.Domain.Inventory.Domain
{
    public class InventoryItemLoaded : IDomainEvent
    {
        public int Quantity { get; set; }
    }
}