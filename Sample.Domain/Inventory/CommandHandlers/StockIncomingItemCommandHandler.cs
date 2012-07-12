using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Client.Commanding;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Inventory;
using Sample.Domain.Inventory.Domain;

namespace Sample.Domain.Inventory.CommandHandlers
{
    public class StockIncomingItemCommandHandler : ICommandHandler<StockIncomingItemCommand>
    {
        private readonly IDebugLogger _logger;
        private readonly IRepository _repository;
        private readonly ICommandSender _commandSender;
        public StockIncomingItemCommandHandler(IDebugLogger logger, IRepository repository, ICommandSender commandSender)
        {
            _logger = logger;
            _repository = repository;
            _commandSender = commandSender;
        }

        public void Handle(StockIncomingItemCommand command)
        {
            Log(string.Format("Received item {0} qty {1}", command.Sku, command.Quantity ));

            var item = _repository.GetById<InventoryItem>(command.ItemId);
            if (!item.HasValidId())
            {
                Log(string.Format("Item {0} Sku {1} is missing.", command.ItemId, command.Sku));
                _commandSender.Send(new CreateInventoryItemCommand(Guid.NewGuid())
                                        {
                                            Description = command.Description,
                                            ItemId = command.ItemId,
                                            Sku = command.Sku
                                        });                
            }
            else
            {
                Log(string.Format("Item {0} Sku {1} found.", command.ItemId, command.Sku));
            }
        }

        private void Log(string message)
        {
            _logger.Log("[inventory] " + message);
        }
    }
}
