using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.Inventory
{
    public class InventoryItem
    {
        public Guid Id { get; protected set; }
        public string Sku { get; protected set; }
        public string Description { get; protected set; }

        public InventoryItem(Guid id, string sku, string description)
        {
            Id = id;
            Sku = sku;
            Description = description;
        }
    }
}
