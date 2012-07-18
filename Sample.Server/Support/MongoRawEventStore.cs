using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Sample.Server.Support
{
    class MongoRawEventStore : IRawEventStore
    {
        private MongoDatabase _db;
        private MongoCollection<PersistedDomainEvent> _collection;

        public MongoRawEventStore(MongoDatabase database)
        {
            _db = database;
            _collection = _db.GetCollection<PersistedDomainEvent>("PersistedDomainEvent");
        }

        public void SaveEvent(PersistedDomainEvent evt)
        {
            _collection.Save(evt);
        }
    }
}
