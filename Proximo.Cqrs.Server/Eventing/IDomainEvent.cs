using Proximo.Cqrs.Server.Aggregates;

namespace Proximo.Cqrs.Server.Eventing
{
    public interface IDomainEvent
    {
        VersionedAggregateId Source { get; }
    }
}
