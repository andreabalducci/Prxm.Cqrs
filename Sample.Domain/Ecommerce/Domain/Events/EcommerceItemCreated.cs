using System;
using CommonDomain;
using Proximo.Cqrs.Server.Aggregates;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Ecommerce.Domain.Events
{
    public class EcommerceItemCreated : DomainEvent
    {
        public string Sku { get; protected set; }
        public string Description { get; protected set; }

        public EcommerceItemCreated(Guid aggregateId, string sku, string description)
        {
            Source = new AggregateVersion(aggregateId,0);
            Sku = sku;
            Description = description;
        }
    }
}