using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.Inventory
{
    public class InventoryItem
    {
        public class Availability
        {
            public string Storage { get; protected set; }
            public decimal Quantity { get; protected set; }

            public Availability(string storage, decimal quantity)
            {
                Storage = storage;
                Quantity = quantity;
            }
        }

        public Guid Id { get; protected set; }
        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public IList<Availability> StorageAvailability { get; protected set; }


        public InventoryItem(Guid id, string sku, string description)
        {
            Id = id;
            Sku = sku;
            Description = description;
            this.StorageAvailability = new List<Availability>();
        }
    }
}
