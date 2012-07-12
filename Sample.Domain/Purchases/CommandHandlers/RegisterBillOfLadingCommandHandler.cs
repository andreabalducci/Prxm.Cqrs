using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Purchases;
using Sample.Domain.Purchases.Domain;

namespace Sample.Domain.Purchases.CommandHandlers
{
    public class RegisterBillOfLadingCommandHandler : ICommandHandler<RegisterBillOfLadingCommand>
    {
        private readonly IRepository _repository;
        private readonly IDebugLogger _debug;

        public RegisterBillOfLadingCommandHandler(IRepository repository, IDebugLogger debug)
        {
            _repository = repository;
            _debug = debug;
        }

        public void Handle(RegisterBillOfLadingCommand command)
        {
            _debug.Log("[purchases] Handling registration of new Bill of lading");
            var bol = new BillOfLading(command.Id, command.Number,command.IssueDate,DateTime.Today);
            
            bol.SetSupplier(command.SupplierCompanyName, command.SupplierAddress);
            foreach (var row in command.Rows)
            {
                bol.AddDetail(row.ItemId, row.Sku, row.Description, row.Quantity);
            }

            bol.Confirm();
            
            _repository.Save(bol,command.Id);
            _debug.Log("[purchases] Bill of lading confirmed");
        }
    }
}
