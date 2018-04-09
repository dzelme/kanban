using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.DbConnection
{
    /// <summary>
    /// A class for making requests to the application's MongoDB database.
    /// </summary>
    public class DbClient : IDbClient
    {
        private MongoClient client;
        private IMongoDatabase db;
        private IMongoCollection<StatisticsDbModel> statisticsCollection;
        private IMongoCollection<BoardPresentationDbModel> presentationCollection;
        private readonly IOptions<DbSettings> dbSettings;

        public DbClient(IOptions<DbSettings> dbSettings)
        {
            this.dbSettings = dbSettings;
            client = new MongoClient(dbSettings.Value.MongoDbUrl);
            db = client.GetDatabase(dbSettings.Value.DatabaseName);
            statisticsCollection = db.GetCollection<StatisticsDbModel>(dbSettings.Value.StatisticsCollectionName);
            presentationCollection = db.GetCollection<BoardPresentationDbModel>(dbSettings.Value.PresentationsCollectionName);
        }

        /// <summary>
        /// Saves information about a Jira REST API request.
        /// </summary>
        /// <param name="entry">An object containing statistics information about the Jira requests.</param>
        /// <returns>The result of the save operation.</returns>
        public Task SaveStatisticsAsync(StatisticsDbModel entry)
        {
            if (entry.Id == null)
                return statisticsCollection.InsertOneAsync(entry);
            return statisticsCollection.ReplaceOneAsync(i => i.Id == entry.Id, entry, new UpdateOptions { IsUpsert = true });
        }

        /// <summary>
        /// Saves a presentation to database.
        /// </summary>
        /// <param name="entry">An object containing presentation data to be saved.</param>
        /// <returns>The result of the save operation.</returns>
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
                return presentationCollection.InsertOneAsync(entryDbModel);
            return presentationCollection.ReplaceOneAsync(i => i.Id == entryDbModel.Id, entryDbModel, new UpdateOptions { IsUpsert = true });
        }

        /// <summary>
        /// Gets a list of all saved presentations.
        /// </summary>
        /// <returns>A list of objects containg information about all presentations saved in database.</returns>
        public async Task<List<BoardPresentationDbModel>> GetPresentationsListAsync()
        {
            var aggregate = presentationCollection
                .Aggregate()
                .Sort(new BsonDocument { { "_id", 1 } });
            var results = await aggregate.ToListAsync();

            return results;
        }

        /// <summary>
        /// Gets board statistics for all boards that have been viewed at least once.
        /// </summary>
        /// <returns>A list of objects containing board view statistics.</returns>
        public async Task<List<StatisticsModel>> GetStatisticsListAsync()
        {
            var aggregate = statisticsCollection
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

        /// <summary>
        /// Gets Jira request statistics for a specified board.
        /// </summary>
        /// <param name="id">The id of the board whose statistics about Jira connections will be obtained.</param>
        /// <returns>A list of objects containing information about requests to Jira REST API for the specified board.</returns>
        public async Task<List<StatisticsConnectionsModel>> GetStatisticsConnectionsListAsync(string id)
        {
            var aggregate = statisticsCollection
                .Aggregate()
                .Match(new BsonDocument {
                    { "Type", "p" },
                    { "BoardId", id }
                })
                .Sort(new BsonDocument { { "Time", -1 } })
                .Limit(100);
            var results = await aggregate.ToListAsync();

            var connectionStatsList = new List<StatisticsConnectionsModel>();
            for (int i = 0; i < results.Count; i++)
            {
                var connectionStatsEntry = new StatisticsConnectionsModel
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

        /// <summary>
        /// Gets all data about a single presentation.
        /// </summary>
        /// <param name="id">The id of the presentation whose data will be obtained.</param>
        /// <returns>An object containing all data stored in the database about the specified presentation.</returns>
        public BoardPresentationDbModel GetAPresentation(string id)
        {
            var filter = Builders<BoardPresentationDbModel>.Filter.Eq("_id", id);
            var document = presentationCollection.Find(filter).FirstOrDefault();  //returns null if no match
            return document;
        }

        /// <summary>
        /// Deletes the specified presentation.
        /// </summary>
        /// <param name="id">The id of the presentation to be deleted.</param>
        public void DeleteAPresentation(string id)
        {
            var filter = Builders<BoardPresentationDbModel>.Filter.Eq("Id", id);
            presentationCollection.DeleteOne(filter);
        }

        /// <summary>
        /// Generates an auto-incremented id for new presentations.
        /// </summary>
        /// <returns>The id for a new presentation.</returns>
        public int GeneratePresentationId()
        {
            var id = (int)presentationCollection.Count(new BsonDocument());
            return ++id;
        }
    }
}
