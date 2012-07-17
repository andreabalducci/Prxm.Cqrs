using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.QueryModel.Rebuilder;
using MongoDB.Driver;

namespace Sample.Server.Support
{
	public class MongoDbDenormalizersHashesStore : IDenormalizersHashesStore
	{
		private MongoDatabase _database;

		protected MongoCollection<DenormalizersHashes> Collecton { get; set; }

		public MongoDbDenormalizersHashesStore(MongoDatabase db)
		{
			_database = db;
			Collecton = _database.GetCollection<DenormalizersHashes>(typeof (DenormalizersHashes).Name.ToLowerInvariant());
		}

		public void Save(DenormalizersHashes hashes)
		{
			Collecton.RemoveAll();
			Collecton.Save(hashes);
		}

		public DenormalizersHashes Load()
		{
			return Collecton.FindOneAs<DenormalizersHashes>();
		}
	}
}
