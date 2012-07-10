using System;
using CommonDomain.Core;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Domain.Inventory.Domain
{
    public class InventoryItem : AggregateBase
    {
        public string ItemId { get; protected set; }
        public string Description { get; protected set; }
        public int Quantity { get; protected set; }

        public InventoryItem()
        {
        }

        public InventoryItem(Guid id, string itemId, string description)
        {
            RaiseEvent(new InventoryItemCreated(){Id = id, ItemId = itemId,ItemDescription = description});
        }

        private void Apply(InventoryItemCreated @event)
        {
            this.Id = @event.Id;
            this.ItemId = @event.ItemId;
            this.Description = @event.ItemDescription;
        }

        public void Load(int i)
        {
            RaiseEvent(new InventoryItemLoaded()
                           {
                               Quantity = i
                           });
        }

        public void Apply(InventoryItemLoaded command)
        {
            Quantity += command.Quantity;
        }
    }
}
