using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain.Core;
using Sample.Domain.Ecommerce.Domain.Events;

namespace Sample.Domain.Ecommerce.Domain
{
    public class EcommerceItem : AggregateBase
    {
        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public decimal UnitPrice { get; protected set; }

        public EcommerceItem()
        {
        }

        public EcommerceItem(Guid id, string sku, string description)
        {
            RaiseEvent(new EcommerceItemCreated(id, sku, description));
        }

        private void Apply(EcommerceItemCreated @event)
        {
            this.Id = @event.Id;
            this.Sku = @event.Sku;
            this.Description = @event.Description;
        }
    }
}
