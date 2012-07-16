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
    public class RegisterBillOfLadingCommandHandler : ICommandHandler
    {
        private readonly IRepository _repository;
        private readonly ILogger _debug;

        public RegisterBillOfLadingCommandHandler(IRepository repository, ILogger debug)
        {
            _repository = repository;
            _debug = debug;
        }

        public void RegisterBillOfLading(RegisterBillOfLadingCommand command)
        {
            _debug.Debug("[purchases] Handling registration of new Bill of lading");
            var bol = new BillOfLading(command.Id, command.Number,command.IssueDate,DateTime.Today);
            
            bol.SetSupplier(command.SupplierCompanyName, command.SupplierAddress);
            foreach (var row in command.Rows)
            {
                bol.AddDetail(row.ItemId, row.Sku, row.Description, row.Quantity);
            }

            bol.Confirm();
            
            _repository.Save(bol,command.Id);
            _debug.Debug("[purchases] Bill of lading confirmed");
        }
    }
}
