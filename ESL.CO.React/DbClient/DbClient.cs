﻿using System;
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
        private readonly IOptions<DbSettings> dbSettings;

        public DbClient(IOptions<DbSettings> dbSettings)
        {
            this.dbSettings = dbSettings;
            client = new MongoClient(dbSettings.Value.MongoDbUrl);
            db = client.GetDatabase(dbSettings.Value.DatabaseName);
            statsCollection = db.GetCollection<StatisticsEntry>(dbSettings.Value.StatisticsCollectionName);
            presCollection = db.GetCollection<BoardPresentation>(dbSettings.Value.PresentationsCollectionName);
            //db.createCollection("collectionName",{capped:true,size:10000,max:5}) //bytes, count
        }

        //public IEnumerable<StatisticsEntry> GetStatisticsList()
        //{
        //    return statsCollection.Find(new BsonDocument()).ToList();
        //    //return db.GetCollection<StatisticsEntry[]>("StatisticsList");
        //}

        //public StatisticsEntry GetStatisticsEntry(string id)
        //{
        //    var filter = Builders<StatisticsEntry>.Filter.Eq("BoardId", id);
        //    var document = statsCollection.Find(filter).FirstOrDefault();  //returns null if no match
        //    return document;
        //    //var res = Query<StatisticsList>.EQ(p => p.Id, id);
        //    //return db.GetCollection<StatisticsList>("StatisticsList").FindOne(res);
        //}

        //public StatisticsEntry SaveStatisticsEntry(StatisticsEntry entry)
        //{
        //    db.GetCollection<StatisticsEntry>("StatisticsList").InsertOne(entry);
        //    return entry;
        //}

        // unnecessary if save can do the same + makes new if doesnt exist
        //public void UpdateStatisticsEntry(StatisticsEntry entry)
        //{
        //    var filter = Builders<StatisticsEntry>.Filter.Eq("BoardId", entry.BoardId);
        //    var update = Builders<StatisticsEntry>.Update
        //        .Set("Name", entry.Name)
        //        .Set("TimesShown", entry.TimesShown)
        //        .Set("LastShown", entry.LastShown)
        //        .Set("NetworkStats", entry.NetworkStats);

        //    statsCollection.UpdateOne(filter, update);


        //    //p.BoardId = id;
        //    //var res = Query<StatisticsList>.EQ(pd => pd.Id, id);
        //    //var operation = Update<StatisticsList>.Replace(p);
        //    //db.GetCollection<StatisticsList>("StatisticsList").Update(res, operation);
        //}

        //public void UpdateNetworkStats(string id, string url, HttpResponseMessage response)
        //{
        //    var entry = GetStatisticsEntry(id);
        //    var networkStats = entry.NetworkStats;
        //    var networkStatsEntry = new JiraConnectionLogEntry(url, response.StatusCode.ToString());

        //    if (networkStats.Count() >= dbSettings.Value.NetworkStatisticsEntryCapacity) { networkStats.Dequeue(); }
        //    networkStats.Enqueue(networkStatsEntry);

        //    UpdateStatisticsEntry(entry);
        //}

        //public void RemoveStatisticsEntry(string id)
        //{
        //    var filter = Builders<StatisticsEntry>.Filter.Eq("BoardId", id);
        //    statsCollection.DeleteOne(filter);

        //    //var res = Query<StatisticsList>.EQ(e => e.Id, id);
        //    //var operation = db.GetCollection<StatisticsList>("StatisticsList").Remove(res);
        //}


        /// <summary>
        /// //////////////////////
        /// </summary>

        //private IMongoCollection<T> ResolveCollectionFromName<T>(string collectionName)
        //{
        //    IMongoCollection<T> collection = null;
        //    if (collectionName == dbSettings.Value.StatisticsCollectionName) { collection = (IMongoCollection<T>)statsCollection; }
        //    if (collectionName == dbSettings.Value.PresentationsCollectionName) { collection = (IMongoCollection<T>)presCollection; }
        //    return collection;
        //}

        private IMongoCollection<T> ResolveCollection<T>()
        {
            IMongoCollection<T> collection = null;
            if (typeof(T).Name == "StatisticsEntry") { collection = (IMongoCollection<T>)statsCollection; }
            if (typeof(T).Name == "BoardPresentation") { collection = (IMongoCollection<T>)presCollection; }
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
            //return db.GetCollection<StatisticsEntry[]>("StatisticsList");
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

            //var res = Query<StatisticsList>.EQ(e => e.Id, id);
            //var operation = db.GetCollection<StatisticsList>("StatisticsList").Remove(res);
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

            Update(id, entry);
        }
    }
}
