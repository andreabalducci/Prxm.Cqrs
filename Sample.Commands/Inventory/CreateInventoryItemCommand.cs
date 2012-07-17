using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Core.Commanding;

namespace Sample.Commands.Inventory
{
    public class CreateInventoryItemCommand : CommandBase
    {
        
        // data
        public Guid ItemId { get; set; }
        public String Sku { get; set; }
        public String Description { get; set; }

        public CreateInventoryItemCommand(Guid id)
        {
            Id = id;
        }

        protected CreateInventoryItemCommand()
        {
        }
    }
}
