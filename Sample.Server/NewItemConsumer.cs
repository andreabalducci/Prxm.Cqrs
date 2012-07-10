using System;
using CommonDomain.Persistence;
using Rhino.ServiceBus;
using Sample.Commands.Inventory;
using Sample.Domain.Inventory.Domain;

namespace Sample.Server
{
    public class NewItemConsumer : ConsumerOf<CreateNewItemCommand>
    {
        private IRepository _repository;

        public NewItemConsumer(IRepository repository)
        {
            _repository = repository;
        }

        public void Consume(CreateNewItemCommand message)
        {
            _repository.Save(new InventoryItem(message.ItemId,message.ItemCode, message.ItemDescription), Guid.NewGuid());
            Console.WriteLine("New item " + message.ItemCode);
        }
    }
}