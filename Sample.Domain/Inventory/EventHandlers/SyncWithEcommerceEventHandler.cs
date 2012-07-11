using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Eventing;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Domain.Inventory.EventHandlers
{
    public class SyncWithEcommerceEventHandler : IDomainEventHandler<InventoryItemCreated>
    {
        protected IDebugLogger _logger;

        public SyncWithEcommerceEventHandler(IDebugLogger logger)
        {
            _logger = logger;
        }

        public void Handle(InventoryItemCreated @event)
        {
            _logger.Log("this will send to Ecommerce bounded context the request for a new item");
        }
    }
}
