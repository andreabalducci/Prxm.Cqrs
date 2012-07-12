using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using Sample.Domain.Inventory.Domain;
using Sample.Domain.Inventory.Domain.Events;
using Sample.Domain.Inventory.EventHandlers;
using Sample.QueryModel.Inventory;
using Sample.QueryModel.Storage.Readers;
using InventoryItem = Sample.QueryModel.Inventory.InventoryItem;

namespace Sample.QueryModel.Builder.Denormalizers.Inventory
{
    public class InventoryItemDenormalizer :
		IDomainEventDenormalizer<InventoryItemCreated>,
		IDomainEventDenormalizer<InventoryItemReceived>

    {
        private IModelWriter<Sample.QueryModel.Inventory.InventoryItem> _itemWriter;
        private IModelWriter<Sample.QueryModel.Inventory.LastReceivedGoods> _lastReceivedGoodsWriter;

        public InventoryItemDenormalizer(IModelWriter<InventoryItem> itemWriter)
        {
            _itemWriter = itemWriter;
        }

        public void Handle(InventoryItemCreated @event)
        {
            _itemWriter.Save(new InventoryItem(@event.Id, @event.Sku,@event.ItemDescription));
        }

        public void Handle(InventoryItemReceived @event)
        {
			
        }
    }
}
