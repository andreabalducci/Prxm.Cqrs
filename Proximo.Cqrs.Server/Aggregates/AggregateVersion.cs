using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Server.Aggregates
{
    public class AggregateVersion 
    {
        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        public AggregateVersion(Guid aggregateId, int version)
        {
            Id = aggregateId;
            Version = version;
        }

        public AggregateVersion Clone()
        {
            return (AggregateVersion)MemberwiseClone();
        }
    }
}
