using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace ESL.CO.React.DbConnection
{
    public class DbClient : IDbClient
    {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<Statistics> statsCollection;
        private IMongoCollection<BoardPresentationDbModel> presCollection;
        private readonly IOptions<DbSettings> dbSettings;

        public DbClient(IOptions<DbSettings> dbSettings)
        {
            this.dbSettings = dbSettings;
            client = new MongoClient(dbSettings.Value.MongoDbUrl);
            db = client.GetDatabase(dbSettings.Value.DatabaseName);
            statsCollection = db.GetCollection<Statistics>(dbSettings.Value.StatisticsCollectionName);
            presCollection = db.GetCollection<BoardPresentationDbModel>(dbSettings.Value.PresentationsCollectionName);
        }

        private IMongoCollection<T> ResolveCollection<T>()
        {
            IMongoCollection<T> collection = null;
            if (typeof(T) == statsCollection.GetType().GetGenericArguments().Single()) { collection = (IMongoCollection<T>)statsCollection; }
            if (typeof(T) == presCollection.GetType().GetGenericArguments().Single()) { collection = (IMongoCollection<T>)presCollection; }

            return collection;
        }

        public Task SaveStatisticsAsync(Statistics entry)
        {
            if (entry.Id == null)
                return statsCollection.InsertOneAsync(entry);
            return statsCollection.ReplaceOneAsync(i => i.Id == entry.Id, entry, new UpdateOptions { IsUpsert = true });
        }

        public Task SavePresentationsAsync(BoardPresentation entry)
        {
            var entryDbModel = new BoardPresentationDbModel
            {
                Id = entry.Id,
                Title = entry.Title,
                Owner = entry.Owner,
                Credentials = entry.Credentials,
                Boards = new List<BoardDbModel>()
            };

            foreach (var board in entry.Boards.Values)
            {
                entryDbModel.Boards.Add(new BoardDbModel
                {
                    Id = board.Id,
                    Visibility = board.Visibility,
                    TimeShown = board.TimeShown,
                    RefreshRate = board.RefreshRate
                });
            }

            if (entryDbModel.Id == null)
                return presCollection.InsertOneAsync(entryDbModel);
            return presCollection.ReplaceOneAsync(i => i.Id == entryDbModel.Id, entryDbModel, new UpdateOptions { IsUpsert = true });
        }

        public async Task<List<BoardPresentationDbModel>> GetPresentationsListAsync()
        {
            var aggregate = presCollection
                .Aggregate()
                .Sort(new BsonDocument { { "_id", 1 } });
                //.Limit(100);
            var results = await aggregate.ToListAsync();

            return results;
        }

        public async Task<List<StatisticsModel>> GetStatisticsListAsync()
        {
            var aggregate = statsCollection
                .Aggregate()
                .Match(new BsonDocument {
                    { "Type", "p" },
                })
                .Group(new BsonDocument {
                    { "_id", "$BoardId" },
                    { "TimesShown", new BsonDocument("$sum", 1)},
                    { "LastShown", new BsonDocument("$last", "$Time")}
                })
                .Sort(new BsonDocument { { "_id", 1 } });
                //.Limit(5);
            var results = await aggregate.ToListAsync();

            var statisticsList = new List<StatisticsModel>();
            for (int i = 0; i < results.Count; i++)
            {
                var statisticsModel = new StatisticsModel
                {
                    BoardId = results[i]["_id"].AsString,
                    TimesShown = results[i]["TimesShown"].AsInt32,
                    LastShown = results[i]["LastShown"].AsString
                };
                statisticsList.Add(statisticsModel);
            }

            return statisticsList;
        }

        public async Task<List<JiraConnectionLogEntry>> GetStatisticsConnectionsListAsync(string id)
        {
            var aggregate = statsCollection
                .Aggregate()
                .Match(new BsonDocument {
                    { "Type", "p" },
                    { "BoardId", id }
                })
                .Sort(new BsonDocument { { "Time", -1 } })
                .Limit(100);
            var results = await aggregate.ToListAsync();

            var connectionStatsList = new List<JiraConnectionLogEntry>();
            for (int i = 0; i < results.Count; i++)
            {
                var connectionStatsEntry = new JiraConnectionLogEntry
                {
                    Time = results.ElementAt(i).Time.ToString(),
                    Link = results.ElementAt(i).Link,
                    ResponseStatus = results.ElementAt(i).ResponseStatus,
                    Exception = results.ElementAt(i).Exception
                };
                connectionStatsList.Add(connectionStatsEntry);
            }

            return connectionStatsList;
        }





        public T GetOne<T>(string id)
        {
            var collection = ResolveCollection<T>();
            var filter = Builders<T>.Filter.Eq("_id", id);
            var document = collection.Find(filter).FirstOrDefault();  //returns null if no match
            return document;
        }

        public void Remove<T>(string id)
        {
            var collection = ResolveCollection<T>();
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }

        public int GeneratePresentationId()
        {
            var id = (int)presCollection.Count(new BsonDocument());
            return ++id;
        }
    }
}
