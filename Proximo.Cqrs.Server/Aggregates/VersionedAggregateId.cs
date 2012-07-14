using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Aggregates
{
    public class VersionedAggregateId
    {
        public Guid AggregateId { get; protected set; }
        public int Version { get; protected set; }

        public VersionedAggregateId(Guid aggregateId, int version)
        {
            AggregateId = aggregateId;
            Version = version;
        }
    }
}
