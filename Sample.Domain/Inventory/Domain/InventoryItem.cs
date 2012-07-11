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

		public void UpdateDescription(string newDescription)
		{
			RaiseEvent(new InventoryItemDescriptionUpdated() { ItemId = this.ItemId, NewDescription = newDescription });
		}

        private void Apply(InventoryItemCreated @event)
        {
            this.Id = @event.Id;
            this.ItemId = @event.ItemId;
            this.Description = @event.ItemDescription;
        }

		private void Apply(InventoryItemDescriptionUpdated @event)
		{
			Description = @event.NewDescription;
		}

        public void Load(int i)
        {
            RaiseEvent(new InventoryItemLoaded()
                           {
                               Quantity = i
                           });
        }

        public void Apply(InventoryItemLoaded @event)
        {
            Quantity += @event.Quantity;
        }
    }
}
