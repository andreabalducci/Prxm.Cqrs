using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Purchases.Domain.Events
{
    /// <summary>
    /// Todo: dispatch a readonly state
    /// </summary>
    public class BillOfLadingConfirmed : IDomainEvent
    {
        public Guid BillOfLadingId { get; protected set; }

        public BillOfLadingConfirmed(Guid id)
        {
            BillOfLadingId = id;
        }
    }
}
