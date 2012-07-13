using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Persistence;
using Proximo.Cqrs.Core.Support;
using Proximo.Cqrs.Server.Commanding;
using Sample.Commands.Inventory;
using Sample.Domain.Inventory.Domain;

namespace Sample.Domain.Inventory.Handlers
{
	/// <summary>
	/// todo: rename this class
	/// </summary>
    public class NewInventoryItemHandler : ICommandHandler
    {
        private IRepository _repository;
        private IDebugLogger _logger;

        public NewInventoryItemHandler(IRepository repository, IDebugLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void CreateInventoryItem(CreateInventoryItemCommand command)
        {
            _logger.Log("[inventory] Creating item " + command.Sku);
            _repository.Save(
                new InventoryItem(command.ItemId, command.Sku, command.Description),
                command.Id
            );
            _logger.Log("[inventory] Item " + command.Sku + " saved");
        }

		public void UpdateInventoryItemDescription(UpdateInventoryItemDescriptionCommand command)
		{
			var aggregate = _repository.GetById<InventoryItem>(command.ItemId);
			_logger.Log(string.Format("[inventory] updating item " + aggregate.ItemId + " description from '" + aggregate.Description + "' to '" + command.Description + "'"));
			aggregate.UpdateDescription(command.Description);
			_repository.Save(aggregate, command.Id);
			_logger.Log("[inventory] Item " + aggregate.ItemId + " saved");
		}
    }
}
