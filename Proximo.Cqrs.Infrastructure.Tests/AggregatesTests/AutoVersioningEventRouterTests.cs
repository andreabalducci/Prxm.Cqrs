using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Proximo.Cqrs.Server.Aggregates;
using Proximo.Cqrs.Server.Eventing;
using Proximo.Cqrs.Server.Impl.Aggregates;

namespace Proximo.Cqrs.Infrastructure.Tests.AggregatesTests
{
    public class SampleAggregate : AggregateRoot
    {
        protected class SomeEvent : DomainEvent
        {
            public Action<DomainEvent> Inspector { get; protected set; }

            public SomeEvent(Action<DomainEvent> inspector)
            {
                Inspector = inspector;
            }
        }

        protected class AnotherEvent : DomainEvent
        {
        }

        protected class Created : DomainEvent
        {
            public Guid Id { get; private set; }

            public Created(Guid id)
            {
                Id = id;
            }
        }

        public SampleAggregate()
        {
        }

        public SampleAggregate(Guid id, Action<DomainEvent> inspector )
        {
            var evt = new Created(id);
            RaiseEvent(evt);
            inspector(evt);
        }

        public void ChangeState(
            Action<DomainEvent> inspectbefore,
            Action<DomainEvent> inspectAfter
        )
        {
            var evt = new SomeEvent(inspectbefore);
            RaiseEvent(evt);
            inspectAfter(evt);
        }

        public void FireAnotherStateChange(
            Action<DomainEvent> inspector
        )
        {
            var evt = new AnotherEvent();
            RaiseEvent(evt);
            inspector(evt);
        }

        protected void Apply(SomeEvent evt)
        {
            evt.Inspector(evt);
        }

        protected void Apply(AnotherEvent evt)
        {
        }

        protected void Apply(Created evt)
        {
            Id = evt.Id;
        }
    }

    [TestFixture]
    public class AutoVersioningEventRouterTests
    {
        [Test]
        public void aggregateroot_should_track_originator()
        {
            var aggregate = new SampleAggregate();
            AggregateVersion before = null;
            AggregateVersion after = null;

            aggregate.ChangeState(
                evt => before = evt.Originator,
                evt => after = evt.Originator
            );

            Assert.IsNull(before);
            Assert.IsNotNull(after);
        }

        [Test]
        public void eventversion_shouldbe_incremented()
        {
            var aggregate = new SampleAggregate();
            AggregateVersion before = null;
            AggregateVersion after = null;

            aggregate.FireAnotherStateChange(
                evt => before = evt.Originator
            );

            aggregate.FireAnotherStateChange(
                evt => after = evt.Originator
            );

            Assert.AreEqual(0, before.Version);
            Assert.AreEqual(1, after.Version);
        }

        [Test]
        public void aggregateId_shouldbe_assigned()
        {
            AggregateVersion version = null;
            var id = Guid.NewGuid();
            new SampleAggregate(id, evt => version = evt.Originator);


            Assert.AreEqual(id, version.Id);
        }
    }
}
