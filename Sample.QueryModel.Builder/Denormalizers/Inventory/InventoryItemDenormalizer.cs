using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Support;
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
        IDomainEventHandler<InventoryItemCreated>,
        IDomainEventHandler<InventoryItemReceived>

    {
        private IModelWriter<Sample.QueryModel.Inventory.InventoryItem> _itemWriter;
        private IModelWriter<Sample.QueryModel.Inventory.LastReceivedGoods> _lastReceivedGoodsWriter;
        private IDebugLogger _logger;
        private IRepository _repository;
        public InventoryItemDenormalizer(IModelWriter<InventoryItem> itemWriter, IDebugLogger logger, IRepository repository)
        {
            _itemWriter = itemWriter;
            _logger = logger;
            _repository = repository;
        }

        public void Handle(InventoryItemCreated @event)
        {
            Log(string.Format("adding {0} to item list", @event.Sku));
            _itemWriter.Save(new InventoryItem(@event.Id, @event.Sku,@event.ItemDescription));
        }

        public void Handle(InventoryItemReceived @event)
        {
            Log(string.Format("updating inventory summary of item {0} ", @event.AggregateId));

        }

        private void Log(string message)
        {
            _logger.Log("[vm-builder] "+message);
        }
    }
}
