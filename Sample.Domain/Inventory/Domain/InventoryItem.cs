using System;
using CommonDomain.Core;
using Proximo.Cqrs.Server.Impl.Aggregates;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Domain.Inventory.Domain
{
    public class InventoryItem : AggregateRoot
    {
        public string ItemId { get; protected set; }
        public string Description { get; protected set; }
        public decimal Quantity { get; protected set; }

        public InventoryItem()
        {
        }

        public InventoryItem(Guid id, string itemId, string description)
        {
            RaiseEvent(new InventoryItemCreated() { Id = id, Sku = itemId, ItemDescription = description });
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

        public void Stock(decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("cannot increase with negative or zero quantity", "quantity");
            }
            RaiseEvent(new InventoryItemStocked(this.Id, quantity));
        }

        public void Apply(InventoryItemStocked @event)
        {
            Quantity += @event.Quantity;
        }

        public void Pick(decimal quantityToPick)
        {
            if (quantityToPick <= 0)
            {
                throw new ArgumentException("cannot pick zero or negative quantity.", "quantity");
            }
            //Business validation
            if (Quantity - quantityToPick < 0)
            {
                RaiseEvent(new InvalidPickingAttempted(
                    Id,
                    InvalidPickingReason.NegativePickingAttempted,
                    Quantity,
                    quantityToPick));
            }
            else
            {
                RaiseEvent(new InventoryItemPicked(this.Id, quantityToPick));
            }
        }

        public void Apply(InventoryItemPicked @event)
        {
            Quantity -= @event.Quantity;
        }

        public void Apply(InvalidPickingAttempted @event)
        {

        }
    }
}
