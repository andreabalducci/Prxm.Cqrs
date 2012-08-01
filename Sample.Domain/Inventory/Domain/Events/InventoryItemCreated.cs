using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemCreated : DomainEvent
    {
        public Guid Id { get; set; }
        public string Sku { get; set; }
        public string ItemDescription { get; set; }

        public override string ToString()
        {
            return "Created an Inventory Item with SKU = " 
                + Sku
                + " Description " 
                + ItemDescription;
        }
    }
}
