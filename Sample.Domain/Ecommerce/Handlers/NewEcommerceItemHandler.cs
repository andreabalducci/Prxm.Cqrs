using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Ecommerce;
using Sample.Domain.Ecommerce.Domain;
using Sample.Infrastructure.Support;

namespace Sample.Domain.Ecommerce.Handlers
{
    public class NewEcommerceItemHandler : ICommandHandler<CreateEcommerceItemCommand>
    {
        private IRepository _repository;
        private IDebugLogger _logger;

        public NewEcommerceItemHandler(IRepository repository, IDebugLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void Handle(CreateEcommerceItemCommand command)
        {
            _logger.Log("[ecommerce] new item " + command.Sku);
            _repository.Save(
                new EcommerceItem(command.ItemId, command.Sku, command.ItemDescription), 
                command.Id
            );
            _logger.Log("[ecommerce] Item " + command.Sku + " saved");
        }
    }
}
