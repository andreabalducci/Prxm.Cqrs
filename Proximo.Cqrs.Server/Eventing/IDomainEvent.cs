using Proximo.Cqrs.Server.Aggregates;

namespace Proximo.Cqrs.Server.Eventing
{
    public interface IDomainEvent
    {
    }

    public interface IVersionedDomainEvent : IDomainEvent
    {
        AggregateVersion Originator { get; }
    }

    public interface IPleaseVersionThisDomainEventAfterInternalStateChange
    {
        void TakeThis(AggregateVersion version);
    }
}
