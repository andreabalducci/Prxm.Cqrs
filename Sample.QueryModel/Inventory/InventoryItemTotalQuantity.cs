using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.Inventory
{
    public class InventoryItemTotalQuantity
    {
        public Guid Id { get; protected set; }

        public String Sku { get; set; }

        public Decimal TotalAvailabilityInAllStorages { get; set; }

        public String Description { get; set; }

        private InventoryItemTotalQuantity() { }

        public InventoryItemTotalQuantity(Guid id)
        {
            Id = id;
        }
    }
}
