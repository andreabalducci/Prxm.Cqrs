using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Infrastructure.Commanding;

namespace Sample.Commands.Inventory
{
    public class CreateInventoryItemCommand : ICommand
    {
        public Guid Id { get; private set; }
        
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
