using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Proximo.Cqrs.Server.Eventing;
using Sample.Domain.Inventory.Domain;
using Sample.Domain.Inventory.Domain.Events;

namespace Sample.Tests.DomainTests.Inventory.InventoryItemTests
{
    [TestFixture]
    public class Cannot_pick_quantity_that_is_not_present_in_inventory_item
        :BaseGwtTest<InventoryItem>
    {
        Guid aggregateId = Guid.NewGuid();

        protected override IEnumerable<Proximo.Cqrs.Server.Eventing.DomainEvent> Given()
        {
            return 
                new DomainEvent[] {
                    new InventoryItemCreated() {Id = aggregateId},
                    new InventoryItemStocked(aggregateId, 20),
                };
        }

        protected override void When(
            InventoryItem aggregateRoot,
            out String explanation)
        {
            explanation = "Picking a quantity of 30";
            aggregateRoot.Pick(30);
        }

        protected override IEnumerable<DomainEvent> ExpectedEvents()
        {
            return
             new DomainEvent[] {
                    new InvalidPickingAttempted(
                        aggregateId, 
                        InvalidPickingReason.NegativePickingAttempted,
                        20, 
                        30),
                };
        }

    }
}
