using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Infrastructure.Commanding;

namespace Sample.Commands.Inventory
{
    public class CreateNewItemCommand : ICommand
    {
        public Guid ItemId { get; set; }
        public String ItemCode { get; set; }
        public String ItemDescription { get; set; }
    }
}
