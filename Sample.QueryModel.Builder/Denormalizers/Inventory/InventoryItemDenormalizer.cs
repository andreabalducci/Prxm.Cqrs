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

using InventoryItem = Sample.QueryModel.Inventory.InventoryItem;

namespace Sample.QueryModel.Builder.Denormalizers.Inventory
{
	/// <summary>
	/// Projects the data in a way that is convenient for the interface
	/// 
	/// if we use these very same denormalizers to also allow for rebuilding they need to provide the logic to update the items too
	/// (even if you are handling creation events).
	/// </summary>
    public class InventoryItemDenormalizer :
        IDomainEventDenormalizer
    {
        private IModelWriter<Sample.QueryModel.Inventory.InventoryItem> _itemWriter;
        private IModelWriter<Sample.QueryModel.Inventory.LastReceivedGoods> _lastReceivedGoodsWriter;
        private ILogger _logger;
        private IRepository _repository;
        public InventoryItemDenormalizer(IModelWriter<InventoryItem> itemWriter, ILogger logger, IRepository repository)
        {
            _itemWriter = itemWriter;
            _logger = logger;
            _repository = repository;
        }

        public void CreateItemOnDenormalizedView(InventoryItemCreated @event)
        {
            Log(string.Format("adding {0} to item list", @event.Sku));
            _itemWriter.Save(new InventoryItem(@event.Id, @event.Sku,@event.ItemDescription));
        }

        public void UpdateQuantityOnReceived(InventoryItemReceived @event)
        {
            Log(string.Format("updating inventory summary of item {0} ", @event.AggregateId));

        }

        private void Log(string message)
        {
            _logger.Debug("[vm-builder] " + message);
        }
    }
}
