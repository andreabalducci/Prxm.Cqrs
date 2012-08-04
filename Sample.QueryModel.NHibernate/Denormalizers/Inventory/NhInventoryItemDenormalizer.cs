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
using NHibernate.Linq;

namespace Sample.QueryModel.Builder.Denormalizers.Inventory
{
    [DenormalizerVersion(2)]
    [EventHandlerDescription(IsSingleton=true)]
    public class NhInventoryItemDenormalizer : BaseDenormalizer
    {
        private static Type[] _denormalizerTypeList = new Type[] {typeof (InventoryItemTotalQuantity)};
        protected override Type[] DenormalizeTypeList
        {
            get { return _denormalizerTypeList; }
        }

        private ILogger _logger;
        private IRepository _repository;

        public NhInventoryItemDenormalizer(ILogger logger, IRepository repository) 
        {
            _logger = logger;
            _repository = repository;
        }

        public void CreateInventoryItem(InventoryItemCreated @event)
        {
            Log(string.Format("Adding Inventory Item SKU={0} to item list", @event.Sku));
            
            var qm = new InventoryItemTotalQuantity(@event.Id);
            qm.TotalAvailabilityInAllStorages = 0.0m;
            qm.Sku = @event.Sku;
            qm.Description = @event.ItemDescription;
           
            //check if exists, it should not but I prefer to be sure
            if (ExecuteInSession(s => s.Query<InventoryItemTotalQuantity>()
                .Count(i => i.Id == qm.Id) > 0))
            {
                Update(qm);
            }
            else {
                Save(qm);
            }
            
        }

        public void UpdateQuantityOnReceived(InventoryItemStocked @event)
        {
            Log(string.Format("updating inventory summary of item {0} ", @event.AggregateId));
            //var aggregate = _repository.GetById<Sample.Domain.Inventory.Domain.InventoryItem>(@event.AggregateId);
            var qm = GetById<InventoryItemTotalQuantity>(@event.AggregateId);
            qm.TotalAvailabilityInAllStorages += @event.Quantity;
            Update(qm);
        }

        public void UpdateQuantityOnPiking(InventoryItemPicked @event)
        {
            Log(string.Format("updating inventory summary of item {0} ", @event.AggregateId));
            //var aggregate = _repository.GetById<Sample.Domain.Inventory.Domain.InventoryItem>(@event.AggregateId);
            var qm = GetById<InventoryItemTotalQuantity>(@event.AggregateId);
            qm.TotalAvailabilityInAllStorages -= @event.Quantity;
            Update(qm);
        }

        private void Log(string message)
        {
            _logger.Debug("[nh-qm-builder] " + message);
        }
    }
}
