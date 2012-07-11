using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Sample.Commands.Ecommerce;
using Sample.Domain.Ecommerce.Domain;
using Sample.Infrastructure.Commanding;
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
            _logger.Log("[ecommerce] new item " + command.ItemCode);
            _repository.Save(
                new EcommerceItem(command.ItemId, command.ItemCode, command.ItemDescription), 
                command.Id
            );
            _logger.Log("[ecommerce] Item " + command.ItemCode + " saved");
        }
    }
}
