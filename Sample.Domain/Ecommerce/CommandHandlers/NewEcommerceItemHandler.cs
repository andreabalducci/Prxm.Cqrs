using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Ecommerce;
using Sample.Domain.Ecommerce.Domain;

namespace Sample.Domain.Ecommerce.CommandHandlers
{
    public class NewEcommerceItemHandler : ICommandHandler<CreateEcommerceItemCommand>
    {
        private readonly IRepository _repository;
        private readonly IDebugLogger _logger;

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
