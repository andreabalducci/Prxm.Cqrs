using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Infrastructure.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemCreated : IDomainEvent
    {
        public Guid Id { get; set; }
        public string ItemId { get; set; }
        public string ItemDescription { get; set; }
    }
}
