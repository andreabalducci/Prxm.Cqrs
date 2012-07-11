using System;
using Sample.Infrastructure.Eventing;

namespace Sample.Domain.Ecommerce.Domain.Events
{
    public class EcommerceItemCreated : IDomainEvent
    {
        public Guid Id { get; protected set; }
        public string ItemCode { get; protected set; }
        public string Description { get; protected set; }

        public EcommerceItemCreated(Guid id, string itemCode, string description)
        {
            Id = id;
            ItemCode = itemCode;
            Description = description;
        }
    }
}