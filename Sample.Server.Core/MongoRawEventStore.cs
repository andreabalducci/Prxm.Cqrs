using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Sample.Server.Support
{
    public class MongoRawEventStore : IRawEventStore
    {
        private MongoDatabase _db;
        private MongoCollection<PersistedDomainEvent> _collection;

        public MongoRawEventStore(MongoDatabase database)
        {
            _db = database;
            _collection = _db.GetCollection<PersistedDomainEvent>("PersistedDomainEvent");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public void SaveEvent(PersistedDomainEvent evt)
        {
            _collection.Save(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventsType"></param>
        /// <returns></returns>
        public IEnumerable<Proximo.Cqrs.Server.Eventing.DomainEvent> LoadEvents(params Type[] eventsType)
        {
            var listOfType = eventsType.Select(t => t.FullName).ToList();
            return _collection.AsQueryable<PersistedDomainEvent>()
                   .Where(evt => listOfType.Contains(evt.EventType))
                   .OrderBy(evt => evt.Timestamp)
                   .Select(evt => evt.DomainEvent);
                   
        }
    }
}
