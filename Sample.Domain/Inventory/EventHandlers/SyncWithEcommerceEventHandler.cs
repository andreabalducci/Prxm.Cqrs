using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Client.Commanding;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Eventing;
using Sample.Commands.Ecommerce;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Domain.Inventory.EventHandlers
{
    public class SyncWithEcommerceEventHandler : IDomainEventHandler<InventoryItemCreated>
    {
        protected readonly IDebugLogger _logger;
        protected readonly ICommandSender _commandSender;

        public SyncWithEcommerceEventHandler(IDebugLogger logger, ICommandSender commandSender)
        {
            _logger = logger;
            _commandSender = commandSender;
        }

        public void Handle(InventoryItemCreated @event)
        {
            _logger.Log("[inventory] Telling ecommerce there's a new item in town");

            var id = Guid.NewGuid();
            _commandSender.Send(new CreateEcommerceItemCommand(id)
                                    {
                                        Sku = @event.Sku,
                                        ItemDescription = @event.ItemDescription,
                                        ItemId = id
                                    });
        }
    }
}
