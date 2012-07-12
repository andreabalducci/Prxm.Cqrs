using System;
using CommonDomain.Core;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Domain.Inventory.Domain
{
    public class InventoryItem : AggregateBase
    {
        public string ItemId { get; protected set; }
        public string Description { get; protected set; }
        public decimal Quantity { get; protected set; }

        public InventoryItem()
        {
        }

        public InventoryItem(Guid id, string itemId, string description)
        {
            RaiseEvent(new InventoryItemCreated(){Id = id, Sku = itemId,ItemDescription = description});
        }

		public void UpdateDescription(string newDescription)
		{
			RaiseEvent(new InventoryItemDescriptionUpdated() { ItemId = this.ItemId, NewDescription = newDescription });
		}

        private void Apply(InventoryItemCreated @event)
        {
            this.Id = @event.Id;
            this.ItemId = @event.Sku;
            this.Description = @event.ItemDescription;
        }

		private void Apply(InventoryItemDescriptionUpdated @event)
		{
			Description = @event.NewDescription;
		}

        public void IncreaseStock(decimal i)
        {
            RaiseEvent(new InventoryItemReceived(this.Id, i));
        }

        public void Apply(InventoryItemReceived @event)
        {
            Quantity += @event.Quantity;
        }
    }
}
