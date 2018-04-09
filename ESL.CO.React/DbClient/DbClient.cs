using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.DbConnection
{
    public class DbClient : IDbClient
    {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<StatisticsEntry> statsCollection;
        private IMongoCollection<BoardPresentation> presCollection;
        private IMongoCollection<Identity> idCollection;
        private IMongoCollection<UserSettingsDbEntry> userSettingsCollection;
        private readonly IOptions<DbSettings> dbSettings;

        public DbClient(IOptions<DbSettings> dbSettings)
        {
            this.dbSettings = dbSettings;
            client = new MongoClient(dbSettings.Value.MongoDbUrl);
            db = client.GetDatabase(dbSettings.Value.DatabaseName);
            statsCollection = db.GetCollection<StatisticsEntry>(dbSettings.Value.StatisticsCollectionName);
            presCollection = db.GetCollection<BoardPresentation>(dbSettings.Value.PresentationsCollectionName);
            idCollection = db.GetCollection<Identity>(dbSettings.Value.IdCollectionName);
            userSettingsCollection = db.GetCollection<UserSettingsDbEntry>(dbSettings.Value.UserSettingsCollectionName);
        }

        private IMongoCollection<T> ResolveCollection<T>()
        {
            IMongoCollection<T> collection = null;
            if (typeof(T) == statsCollection.GetType().GetGenericArguments().Single()) { collection = (IMongoCollection<T>)statsCollection; }
            if (typeof(T) == presCollection.GetType().GetGenericArguments().Single()) { collection = (IMongoCollection<T>)presCollection; }
            if (typeof(T) == userSettingsCollection.GetType().GetGenericArguments().Single()) { collection = (IMongoCollection<T>)userSettingsCollection; }

            return collection;
        }

        public T Save<T>(T entry)
        {
            var collection = ResolveCollection<T>();
            collection.InsertOne(entry);
            return entry;
        }

        public IEnumerable<T> GetList<T>()
        {
            var collection = ResolveCollection<T>();
            return collection.Find(new BsonDocument()).ToList();
        }

        public T GetOne<T>(string id)
        {
            var collection = ResolveCollection<T>();
            var filter = Builders<T>.Filter.Eq("Id", id);
            var document = collection.Find(filter).FirstOrDefault();  //returns null if no match
            return document;
        }

        public void Update<T>(string id, T entry)
        {
            Remove<T>(id);
            Save<T>(entry);
        }

        public void Remove<T>(string id)
        {
            var collection = ResolveCollection<T>();
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }

        public void UpdateNetworkStats(string id, string url, HttpResponseMessage response)
        {
            var entry = GetOne<StatisticsEntry>(id);
            if(entry == null) { return; }  // starts to save network stats only after the board has been shown once

            var networkStats = entry.NetworkStats;
            var networkStatsEntry = new JiraConnectionLogEntry(url, response.StatusCode.ToString());

            while (networkStats.Count() >= dbSettings.Value.NetworkStatisticsEntryCapacity)
            {
                networkStats.Dequeue();
            }
            networkStats.Enqueue(networkStatsEntry);

            //Update(id, entry);
        }

        public int GeneratePresentationId()
        {

            var filter = Builders<Identity>.Filter.Eq("Id", "IncrementId");
            var identity = idCollection.Find(filter).FirstOrDefault();
            if (identity == null)
            {
                identity = new Identity
                {
                    Id = "IncrementId",
                    SequenceValue = 0
                };
                idCollection.InsertOne(identity);
            }

            var update = Builders<Identity>.Update
                .Set("SequenceValue", ++identity.SequenceValue);  //
            idCollection.UpdateOne(filter, update);

            return identity.SequenceValue;
        }
    }
}
