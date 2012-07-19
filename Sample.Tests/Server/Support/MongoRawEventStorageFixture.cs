using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Tests.TestInfrastructure;
using NUnit.Framework;
using MongoDB.Driver;
using Proximo.Cqrs.Server.Eventing;
using Sample.Server.Support;
using SharpTestsEx;

namespace Sample.Tests.Server.Support
{
    [TestFixture]
    public class MongoRawEventStorageFixture : BaseTestFixtureWithHelper
    {
        MongoDatabase database;

        MongoRawEventStore sut;

        protected override void OnTestFixtureSetUp()
        {
            base.OnTestFixtureSetUp();
            database = MongoServer.Create().GetDatabase("cqrs-fixture-test");
        }
        protected override void OnSetUp()
        {
            base.OnSetUp();
            database.GetCollection("PersistedDomainEvent").Drop();
            sut = new MongoRawEventStore(database);
        }

        [Test]
        public void Verify_basic_save_of_an_event()
        {
            Dea a = new Dea() { Paperoga = "Yes", Test = 2 };
            Deb b = new Deb() { BlaBlaList = new List<String>() { "one", "two", "three" }, Number = 34.5 };

            PersistedDomainEvent evta = new PersistedDomainEvent()
            {
                CommitSequence = 1,
                DomainEvent = a,
                EventType = a.GetType().FullName,
                Timestamp = DateTime.Now,
            };
            sut.SaveEvent(evta);

            var single = sut.LoadEvents(a.GetType()).Single();
            single.Should().Be.OfType<Dea>();
        }
    }

    public class Dea : DomainEvent {

        public String Paperoga { get; set; }

        public Int32 Test { get; set; }
    }

    public class Deb : DomainEvent
    {
        public List<String> BlaBlaList { get; set; }

        public Double Number { get; set; }
    }
}
