using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Ecommerce;
using Sample.Domain.Ecommerce.Domain;

namespace Sample.Domain.Ecommerce.CommandHandlers
{
    public class NewEcommerceItemHandler : ICommandHandler
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public NewEcommerceItemHandler(IRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void CreateECommerce(CreateEcommerceItemCommand command)
        {
            _logger.Debug("[ecommerce] new item " + command.Sku);
            _repository.Save(
                new EcommerceItem(command.ItemId, command.Sku, command.ItemDescription), 
                command.Id
            );
            _logger.Debug("[ecommerce] Item " + command.Sku + " saved");
        }

    }
}
