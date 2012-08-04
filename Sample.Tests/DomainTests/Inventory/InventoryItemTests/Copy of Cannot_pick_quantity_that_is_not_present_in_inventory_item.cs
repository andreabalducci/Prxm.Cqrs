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
    public class Can_pick_quantity_if_enough_is_present_in_stock
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
            aggregateRoot.Pick(10);
        }

        protected override IEnumerable<DomainEvent> ExpectedEvents()
        {
            return 
             new DomainEvent[] {
                    new InventoryItemPicked(
                        aggregateId, 
                        10),
                };
        }

    }
}
