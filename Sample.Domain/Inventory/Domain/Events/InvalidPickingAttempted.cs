using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.Domain.Inventory.Domain.Events
{
    public class InvalidPickingAttempted : DomainEvent
    {
        public Guid AggregateId { get; set; }

        public Decimal ActualQuantity { get; set; }

        public Decimal RequestedQuantity { get; set; }

        public InvalidPickingReason Reason { get; set; }


        public InvalidPickingAttempted(
            Guid aggreateId,
            InvalidPickingReason reason,
            Decimal actualQuantity,
            Decimal requestedQuantity) {

                AggregateId = aggreateId;
                Reason = reason;
                RequestedQuantity = requestedQuantity;
                ActualQuantity = actualQuantity;
        }

        public override string ToString()
        {
            return "Invalid picking attempt of quantity " +
                RequestedQuantity +
                " on an item with stocked quantity " +
                ActualQuantity + ". Reason " +
                Reason;
        }
    }

    public enum InvalidPickingReason 
    {
        Unknown,
        NegativePickingAttempted,
        TresholdViolation,

    }

}
