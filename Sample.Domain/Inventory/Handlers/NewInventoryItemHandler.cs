using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Sample.Commands.Inventory;
using Sample.Domain.Inventory.Domain;
using Sample.Infrastructure.Commanding;
using Sample.Infrastructure.Support;

namespace Sample.Domain.Inventory.Handlers
{
    public class NewInventoryItemHandler : ICommandHandler<CreateInventoryItemCommand>
    {
        private IRepository _repository;
        private IDebugLogger _logger;

        public NewInventoryItemHandler(IRepository repository, IDebugLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void Handle(CreateInventoryItemCommand command)
        {
            _logger.Log("[inventory] Creating item " + command.Sku);
            _repository.Save(
                new InventoryItem(command.ItemId, command.Sku, command.Description),
                command.Id
            );
            _logger.Log("[inventory] Item " + command.Sku + " saved");
        }
    }
}
