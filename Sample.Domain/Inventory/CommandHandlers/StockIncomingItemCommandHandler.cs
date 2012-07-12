using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Inventory;

namespace Sample.Domain.Inventory.CommandHandlers
{
    public class StockIncomingItemCommandHandler : ICommandHandler<StockIncomingItemCommand>
    {
        private IDebugLogger _logger;

        public StockIncomingItemCommandHandler(IDebugLogger logger)
        {
            _logger = logger;
        }

        public void Handle(StockIncomingItemCommand command)
        {
            _logger.Log(string.Format("[inventory] Received item {0} qty {1}", command.Sku, command.Quantity ));
        }
    }
}
