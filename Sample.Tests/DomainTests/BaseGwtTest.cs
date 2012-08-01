using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using NUnit.Framework;
using Proximo.Cqrs.Server.Eventing;
using Proximo.Cqrs.Server.Impl.Aggregates;
using Sample.Tests.TestInfrastructure;

namespace Sample.Tests.DomainTests
{
    [TestFixture]
    public abstract class BaseGwtTest<T> where T : AggregateRoot
    {
        /// <summary>
        /// Gives me the event to move the AR to when state
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<DomainEvent> Given();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregateRoot"></param>
        protected abstract void When(T aggregateRoot, out String explanation);

        /// <summary>
        /// list of expected events.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<DomainEvent> ExpectedEvents();

        protected virtual Action<Exception> ExceptionVerifier() 
        {
            return null;
        }

        protected Exception caught;

        private StringBuilder gwtText = new StringBuilder();

        [Test]
        public void Exercise()
        {
            gwtText.AppendLine("Specification: \n\t" + this.GetType().Name.Replace("_", " "));
            var exceptionVerifier = ExceptionVerifier();
            try
            {
                var events = Given();
                gwtText.AppendLine("Given:");
                foreach (var @event in events)
                {
                    gwtText.AppendLine("\t" + @event.ToString());
                }
                T aggregateRoot = Activator.CreateInstance<T>();
                foreach (var @event in events)
                {
                    ((IAggregate)aggregateRoot).ApplyEvent(@event);
                }

                var expectedEvents = ExpectedEvents();
                String whenDescription;
                When(aggregateRoot, out whenDescription);
                gwtText.AppendLine("When: \n\t" + whenDescription);

                var raisedEvents = ((IAggregate)aggregateRoot)
                    .GetUncommittedEvents()
                    .Cast<DomainEvent>();
                gwtText.AppendLine("Expect:");
                foreach (var @event in expectedEvents)
                {
                    gwtText.AppendLine("\t" + @event.ToString());
                }
                if (raisedEvents.Count() != expectedEvents.Count())
                {
                    Assert.Fail("Different number of events returned");
                }
                for (int i = 0; i < raisedEvents.Count(); i++)
                {
                    Assert.That(GenericEquals.Equals(
                        raisedEvents.ElementAt(i),
                        expectedEvents.ElementAt(i)));
                }
            }
            catch (Exception ex)
            {
                caught = ex;
                if (exceptionVerifier == null) throw;
            }
 
            if (exceptionVerifier != null) 
            {
                exceptionVerifier(caught);
            }
            if (exceptionVerifier != null && caught == null) 
            {
                Assert.Fail("Exception expected but not raised");
            }
            Console.WriteLine(gwtText.ToString());
        }

    }
}
