using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Eventing;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Domain.Inventory.EventHandlers
{
    public class NewInventoryItemCreatedEventHandler : IDomainEventHandler
    {
        protected ILogger _logger;

        public NewInventoryItemCreatedEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void ReactToInventoryItemCreated(InventoryItemCreated @event)
        {
            _logger.Debug(string.Format("[inventory] item {0} has been created and handled", @event.Sku));
        }
    }
}
