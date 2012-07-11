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
        public string ItemId { get; protected set; }
        public string Description { get; protected set; }
        public decimal UnitPrice { get; protected set; }

        public EcommerceItem()
        {
        }

        public EcommerceItem(Guid id, string itemCode, string description)
        {
            RaiseEvent(new EcommerceItemCreated(id, itemCode, description));
        }

        private void Apply(EcommerceItemCreated @event)
        {
            this.Id = @event.Id;
            this.ItemId = @event.ItemCode;
            this.Description = @event.Description;
        }
    }
}
