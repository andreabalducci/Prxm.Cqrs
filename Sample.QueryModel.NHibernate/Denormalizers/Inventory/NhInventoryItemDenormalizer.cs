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
using NHibernate;
using Sample.QueryModel.NHibernate;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.QueryModel.Builder.Denormalizers.Inventory
{
    [CurrentDenormalizerVersion(4)]
    [EventHandlerDescription(IsSingleton=true)]
    public class NhInventoryItemDenormalizer : BaseDenormalizer
    {
        private ILogger _logger;
        private IRepository _repository;

        public NhInventoryItemDenormalizer(ILogger logger, IRepository repository, ICommandQueue commandQueue) : base (commandQueue)
        {
            _logger = logger;
            _repository = repository;
        }

        public void CreateItemOnDenormalizedView(InventoryItemCreated @event)
        {
            Log(string.Format("adding {0} to item list", @event.Sku));
            var qm = GetById<InventoryItemTotalQuantity>(@event.Id);
            if (qm == null) {
                qm = new InventoryItemTotalQuantity(@event.Id);
            }
            qm.TotalAvailabilityInAllStorages = 0.0m;
            qm.Sku = @event.Sku;
            qm.Description = @event.ItemDescription;
            SaveOrUpdate(qm);
        }

        public void UpdateQuantityOnReceived(InventoryItemReceived @event)
        {
            Log(string.Format("updating inventory summary of item {0} ", @event.AggregateId));
            //var aggregate = _repository.GetById<Sample.Domain.Inventory.Domain.InventoryItem>(@event.AggregateId);
            var qm = GetById<InventoryItemTotalQuantity>(@event.AggregateId);
            qm.TotalAvailabilityInAllStorages += @event.Quantity;
            SaveOrUpdate(qm);
        }

        private void Log(string message)
        {
            _logger.Debug("[nh-qm-builder] " + message);
        }
    }
}
