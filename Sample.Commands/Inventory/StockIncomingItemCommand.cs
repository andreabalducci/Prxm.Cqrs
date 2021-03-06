﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.Inventory
{
    public class StockIncomingItemCommand : CommandBase
    {

        /// <summary>
        /// The id of the item 
        /// </summary>
        public Guid ItemId { get; protected set; }

        /// <summary>
        /// Sku of the item
        /// </summary>
        public string Sku { get; protected set; }

        public string Description { get; protected set; }

        public decimal Quantity { get; protected set; }

        public string Storage { get; protected set; }

        protected StockIncomingItemCommand()
        {
        }

        public StockIncomingItemCommand(Guid commandId, Guid itemId, string sku, string description, decimal quantity, string storage)
        {
            Id = commandId;
            this.ItemId = itemId;
            Sku = sku;
            Description = description;
            Quantity = quantity;
            Storage = storage;
        }
    }
}
