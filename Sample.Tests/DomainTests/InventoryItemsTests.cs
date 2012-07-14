using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using EventStore;
using EventStore.Serialization;
using MongoDB.Bson.Serialization;
using NUnit.Framework;
using Sample.Domain.Inventory;
using Sample.Domain.Inventory.Domain;
using Sample.Domain.Inventory.Domain.Events;

// ReSharper disable InconsistentNaming

namespace Sample.Tests.DomainTests
{
    public class InnerBuilder : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var a = Activator.CreateInstance(type) as IAggregate;

            return a;
        }
    }

    [TestFixture]
    public class InventoryItemsTests
    {
        private Guid id = Guid.Parse("{022744D8-DAA7-4F36-B311-DF70F35B26FB}");
        private static IStoreEvents _store;

        [TestFixtureSetUp]
        public void Setup()
        {
            BsonClassMap.RegisterClassMap<InventoryItemCreated>();
            BsonClassMap.RegisterClassMap<InventoryItemReceived>();

            _store = Wireup.Init()
                .UsingMongoPersistence("demo", new DocumentObjectSerializer())
                .InitializeStorageEngine()
                .Build();
        }

        [Test]
        public void can_create_new_item()
        {
            var item = new InventoryItem(
                id,
                "001",
                "Sample Item"
            );

            Assert.NotNull(item.ItemId);
        }

        [Test,Explicit]
        public void save_to_storage()
        {
            var item = new InventoryItem(
                id,
                "001",
                "Sample Item"
            );

            using (var r = BuildRepository())
            {
                r.Save(item, Guid.NewGuid());
            }
        }

        private EventStoreRepository BuildRepository()
        {
            return new EventStoreRepository(_store, new InnerBuilder(), new ConflictDetector());
        }

        [Test,Explicit]
        public void load_from_storage()
        {
            using (var r = BuildRepository())
            {
                var item = r.GetById<InventoryItem>(id);

                Assert.AreEqual(id, item.Id);
                Assert.AreEqual("Sample Item", item.Description);
            }
        }

        [Test,Explicit]
        public void receive_items()
        {
            decimal currentQty = 0;
            using (var r = BuildRepository())
            {
                var item = r.GetById<InventoryItem>(id);
                currentQty = item.Quantity;
                item.IncreaseStock(100);
                r.Save(item, Guid.NewGuid());
            }

            using (var r = BuildRepository())
            {
                var item = r.GetById<InventoryItem>(id);
                Assert.AreEqual(currentQty + 100, item.Quantity);
            }
        }

        [Test,Explicit]
        public void massive_save_to_storage()
        {
            var s = new Stopwatch();
            s.Start();
            Parallel.ForEach(Enumerable.Range(1, 10000), x =>
                                                             {
                                                                 using (var r = BuildRepository())
                                                                 {
                                                                     var item = new InventoryItem(
                                                                         Guid.NewGuid(),
                                                                         "001",
                                                                         "Sample Item"
                                                                     );

                                                                     r.Save(item, item.Id);
                                                                 }
                                                             });
            s.Stop();
            Debug.WriteLine("Elapsed " + s.ElapsedMilliseconds / 1000.0);
        }
    }
}
