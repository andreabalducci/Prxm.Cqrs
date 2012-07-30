using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.Saga.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace Sample.Saga.Impl
{
	/// <summary>
	/// quick and dirty implementation of a repository to persist the saga state.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TId"></typeparam>
	public class MongoDbSagaRepository<T, TId> : ISagaRepository<T, TId> where T:SagaState<TId>, new()
	{
		public readonly MongoDatabase _database;
		
		protected MongoCollection<T> Collecton { get; set; }

		private string CollectionName
		{
			get { return typeof(T).Name.ToLowerInvariant(); }
		}

		public MongoDbSagaRepository(MongoDatabase database)
        {
            _database = database;

            Collecton = _database.GetCollection<T>(CollectionName);
        }

		public T Load(IDictionary<string, object> @params)
		{
			// build up the query
			IMongoQuery[] q = new IMongoQuery[@params.Keys.Count];
			int idx = 0;
			foreach (var de in @params)
			{
				// MongoDb maps 'Id' to '_id'
				string key = de.Key;
				// ugly fix
				if (key == "Id")
					key = "_id";
				q[idx++] = Query.EQ(key, BsonTypeMapper.MapToBsonValue(de.Value));
			}
			var mongoQ = Query.And(q);
			var result = Collecton.FindOneAs<T>(mongoQ);
			return result;
		}

		public void Save(T state)
		{
			Collecton.Save(state);
		}

		public void Remove(T state)
		{
			var q = Query.EQ("_id", BsonTypeMapper.MapToBsonValue(state.Id));
			Collecton.Remove(q);
		}
	}
}
