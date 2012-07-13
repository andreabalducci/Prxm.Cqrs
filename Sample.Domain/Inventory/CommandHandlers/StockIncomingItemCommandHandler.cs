using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Commanding;
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

            //
            // Create the item if not found (it's just a sample, should not be applied to a real system)
            // 
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
            
                // push back the command (disclaimer: assuming a single thread processor)
                // maybe we should change che command id? (check event store concurrency)
                Log("Requeuing incoming stock request");
                _commandSender.Send(command);
                return;
            }
            
            item.IncreaseStock(command.Quantity);
            Log(string.Format("Item {0} +{1} ", command.Sku, command.Quantity));
        }

        private void Log(string message)
        {
            _logger.Log("[inventory] " + message);
        }
    }
}
