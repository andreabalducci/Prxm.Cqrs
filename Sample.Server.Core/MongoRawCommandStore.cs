using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Sample.Server.Core;

namespace Sample.Server.Core
{
    public class MongoRawCommandStore : IRawCommandStore
    {
        private MongoDatabase _db;
        private MongoCollection<PersistedDomainEvent> _collection;

        public MongoRawCommandStore(MongoDatabase database)
        {
            _db = database;
            _collection = _db.GetCollection<PersistedDomainEvent>("RawCommandStore");
        }



        public void Store(ExecutedCommand executedCommand)
        {
            _collection.Save(executedCommand);
        }
    }
}
