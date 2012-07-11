using System;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Ecommerce.Domain.Events
{
    public class EcommerceItemCreated : IDomainEvent
    {
        public Guid Id { get; protected set; }
        public string Sku { get; protected set; }
        public string Description { get; protected set; }

        public EcommerceItemCreated(Guid id, string sku, string description)
        {
            Id = id;
            Sku = sku;
            Description = description;
        }
    }
}