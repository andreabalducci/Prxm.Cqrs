using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Infrastructure.Commanding;

namespace Sample.Commands.Ecommerce
{
    public class CreateEcommerceItemCommand : ICommand
    {
        public Guid Id { get; private set; }
        public Guid ItemId { get; set; }
        public String Sku { get; set; }
        public String ItemDescription { get; set; }

        public CreateEcommerceItemCommand(Guid id)
        {
            Id = id;
        }

        protected CreateEcommerceItemCommand()
        {
        }
    }
}
