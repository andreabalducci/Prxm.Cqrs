using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InventoryItemDescriptionUpdated : DomainEvent
	{
		public string ItemId { get; set; }
		public string NewDescription { get; set; }
	}
}
