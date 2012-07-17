using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace Sample.QueryModel.Builder
{
    public class ModelWriter<T> : IModelWriter<T>
    {
        public readonly MongoDatabase _database;
        protected MongoCollection<T> Collecton { get; set; }

        public ModelWriter(MongoDatabase database)
        {
            _database = database;

            Collecton = _database.GetCollection<T>(CollectionName);
        }

        private string CollectionName
        {
            get { return typeof (T).Name.ToLowerInvariant(); }
        }

        public void Save(T model)
        {
            Collecton.Save(model);
        }
    }        
}
