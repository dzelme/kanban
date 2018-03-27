using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ESL.CO.React.DbConnection
{
    public class DbClient : IDbClient
    {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<StatisticsEntry> collection;

        public DbClient()
        {
            client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("StatisticsDB");
            collection = db.GetCollection<StatisticsEntry>("StatisticsList");
            //db.createCollection("collectionName",{capped:true,size:10000,max:5}) //bytes, count
        }

        public IEnumerable<StatisticsEntry> GetStatisticsList()
        {
            return collection.Find(new BsonDocument()).ToList();
            //return db.GetCollection<StatisticsEntry[]>("StatisticsList");
        }

        public StatisticsEntry GetStatisticsEntry(string id)
        {
            var filter = Builders<StatisticsEntry>.Filter.Eq("BoardId", id);
            var document = collection.Find(filter).FirstOrDefault();  //returns null if no match
            return document;
            //var res = Query<StatisticsList>.EQ(p => p.Id, id);
            //return db.GetCollection<StatisticsList>("StatisticsList").FindOne(res);
        }

        public StatisticsEntry SaveStatisticsEntry(StatisticsEntry entry)
        {
            db.GetCollection<StatisticsEntry>("StatisticsList").InsertOne(entry);
            return entry;
        }

        // unnecessary if save can do the same + makes new if doesnt exist
        public void UpdateStatisticsEntry(string id, StatisticsEntry entry)
        {
            var filter = Builders<StatisticsEntry>.Filter.Eq("BoardId", id);
            var update = Builders<StatisticsEntry>.Update
                .Set("Name", entry.Name)
                .Set("TimesShown", entry.TimesShown)
                .Set("LastShown", entry.LastShown)
                .Set("NetworkStats", entry.NetworkStats);

            collection.UpdateOne(filter, update);


            //p.BoardId = id;
            //var res = Query<StatisticsList>.EQ(pd => pd.Id, id);
            //var operation = Update<StatisticsList>.Replace(p);
            //db.GetCollection<StatisticsList>("StatisticsList").Update(res, operation);
        }

        //public void UpdateNetworkStats(string id)
        //{

        //}

        public void RemoveStatisticsEntry(string id)
        {
            var filter = Builders<StatisticsEntry>.Filter.Eq("BoardId", id);
            collection.DeleteOne(filter);

            //var res = Query<StatisticsList>.EQ(e => e.Id, id);
            //var operation = db.GetCollection<StatisticsList>("StatisticsList").Remove(res);
        }
    }
}
