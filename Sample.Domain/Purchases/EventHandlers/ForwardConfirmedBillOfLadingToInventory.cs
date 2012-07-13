using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Commanding;
using Proximo.Cqrs.Server.Eventing;
using Sample.Commands.Inventory;
using Sample.Domain.Purchases.Domain;
using Sample.Domain.Purchases.Domain.Events;

namespace Sample.Domain.Purchases.EventHandlers
{
    public class ForwardConfirmedBillOfLadingToInventory : IDomainEventHandler<BillOfLadingConfirmed>
    {
        private readonly ICommandSender _commandSender;
        private readonly IRepository _repository;
        private const string IncomingGoodsStorage = "QC";

        public ForwardConfirmedBillOfLadingToInventory(ICommandSender commandSender, IRepository repository)
        {
            _commandSender = commandSender;
            _repository = repository;
        }

        public void Handle(BillOfLadingConfirmed @event)
        {
            var bol = _repository.GetById<BillOfLading>(@event.BillOfLadingId);

            foreach (var detail in bol.Details)
            {
                var cmd = new StockIncomingItemCommand(Guid.NewGuid(), 
                    detail.ItemId,
                    detail.Sku, 
                    detail.Description, 
                    detail.Quantity,
                    IncomingGoodsStorage
                );
                
                _commandSender.Send(cmd);
            }
        }
    }
}
