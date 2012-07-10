using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Commands.Inventory;
using Sample.Infrastructure.Commanding;

namespace Sample.Domain.Inventory.Handlers
{
    public class NewInventoryItemHandler : IHandler<CreateNewItemCommand>
    {
        public void Handle(CreateNewItemCommand command)
        {

        }
    }
}
