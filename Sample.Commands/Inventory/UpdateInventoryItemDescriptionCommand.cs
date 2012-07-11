using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.Inventory
{
	public class UpdateInventoryItemDescriptionCommand : ICommand
	{
		public Guid Id { get; private set; }

		// data
		public Guid ItemId { get; set; }
		public String Description { get; set; }

		public UpdateInventoryItemDescriptionCommand(Guid id)
		{
			Id = id;
		}

		protected UpdateInventoryItemDescriptionCommand()
        {
        }
	}
}
