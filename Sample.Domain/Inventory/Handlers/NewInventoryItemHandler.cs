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
    public class NewInventoryItemHandler : ICommandHandler<CreateNewItemCommand>
    {
        private IRepository _repository;
        private IDebugLogger _logger;

        public NewInventoryItemHandler(IRepository repository, IDebugLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void Handle(CreateNewItemCommand command)
        {
            _logger.Log("Creating item "+ command.ItemCode);
            _repository.Save(
                new InventoryItem(command.ItemId, command.ItemCode, command.ItemDescription),
                command.Id
            );
            _logger.Log("Item " + command.ItemCode + " saved");
        }
    }
}
