using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.Inventory
{
    public class StockIncomingItemCommand : ICommand
    {
        public Guid Id { get; protected set; }

        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public decimal Quantity { get; protected set; }
        public string Storage { get; protected set; }

        public StockIncomingItemCommand(Guid id, string sku, string description, decimal quantity, string storage)
        {
            Id = id;
            Sku = sku;
            Description = description;
            Quantity = quantity;
            Storage = storage;
        }
    }
}
