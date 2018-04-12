using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace ESL.CO.React.DbConnection
{
    /// <summary>
    /// A class for making requests to the application's MongoDB database.
    /// </summary>
    public class DbClient : IDbClient
    {
        private MongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<StatisticsDbModel> statisticsCollection;
        private IMongoCollection<BoardPresentationDbModel> presentationCollection;
        private readonly IOptions<DbSettings> dbSettings;

        public DbClient(IOptions<DbSettings> dbSettings)
        {
            this.dbSettings = dbSettings;
            client = new MongoClient(dbSettings.Value.MongoDbUrl);
            database = client.GetDatabase(dbSettings.Value.DatabaseName);
            statisticsCollection = database.GetCollection<StatisticsDbModel>(dbSettings.Value.StatisticsCollectionName);
            presentationCollection = database.GetCollection<BoardPresentationDbModel>(dbSettings.Value.PresentationsCollectionName);
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
            if (string.IsNullOrEmpty(entry.Id))
            {
                entry.Id = GeneratePresentationId().ToString();
            }

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
            var results = await presentationCollection
                .Find(FilterDefinition<BoardPresentationDbModel>.Empty)
                .Sort(new BsonDocument { { "_id", 1 } })
                .ToListAsync();

            return results;
        }

        /// <summary>
        /// Gets view statistics for all presentations that have been viewed at least once.
        /// </summary>
        /// <returns>A list of objects containing presentation view statistics.</returns>
        public async Task<IEnumerable<StatisticsPresentationModel>> GetStatisticsPresentationListAsync()
        {
            var aggregate = statisticsCollection
                .Aggregate()
                .Match(new BsonDocument {
                    { "Type", "presentation" },
                })
                .Group(new BsonDocument {
                    { "_id", "$PresentationId" },
                    { "TimesShown", new BsonDocument("$sum", 1)},
                    { "LastShown", new BsonDocument("$last", "$Time")}
                })
                .Sort(new BsonDocument { { "_id", 1 } })
                .Project(new BsonDocument
                {
                    { "_id", 0 },
                    { "PresentationId", "$_id" },
                    { "TimesShown", "$TimesShown" },
                    { "LastShown","$LastShown" }
                });

            var results = await aggregate.ToListAsync();

            var statisticsPresentationList = new List<StatisticsPresentationModel>();
            foreach (var item in results)
            {
                statisticsPresentationList.Add(BsonSerializer.Deserialize<StatisticsPresentationModel>(item));
            }

            return statisticsPresentationList;
        }

        /// <summary>
        /// Gets presentation specific view statistics for all boards that have been viewed as part of the specified presentation at least once.
        /// </summary>
        /// <param name="presentationId">The id of the presentation whose board statistics will be obtained.</param>
        /// <returns>A list of objects containing board view statistics.</returns>
        public async Task<IEnumerable<StatisticsBoardModel>> GetStatisticsBoardListAsync(string presentationId)
        {
            var aggregate = statisticsCollection
                .Aggregate()
                .Match(new BsonDocument {
                    { "Type", "board" },
                    { "PresentationId", presentationId }
                })
                .Group(new BsonDocument {
                    { "_id", "$BoardId" },
                    { "TimesShown", new BsonDocument("$sum", 1)},
                    { "LastShown", new BsonDocument("$last", "$Time")}
                })
                .Sort(new BsonDocument { { "_id", 1 } })
                .Project(new BsonDocument
                {
                    { "_id", 0 },
                    { "BoardId", "$_id" },
                    { "TimesShown", "$TimesShown" },
                    { "LastShown","$LastShown" }
                });
            
            var results = await aggregate.ToListAsync();

            var statisticsList = new List<StatisticsBoardModel>();
            foreach (var item in results)
            {
                statisticsList.Add(BsonSerializer.Deserialize<StatisticsBoardModel>(item));
            }

            return statisticsList;
        }

        /// <summary>
        /// Gets presentation specific Jira request statistics for a specified board as part of the specified presentation.
        /// </summary>
        /// <param name="presentationId">The id of the presentation containing the board whose Jira connections statistics will be obtained.</param>
        /// <param name="boardId">The id of the board whose Jira connections statistics will be obtained.</param>
        /// <returns>A list of objects containing information about requests to Jira REST API for the specified board.</returns>
        public async Task<List<StatisticsConnectionModel>> GetStatisticsConnectionsListAsync(string presentationId, string boardId)
        {
            var filter =
                Builders<StatisticsDbModel>.Filter.Eq("PresentationId", presentationId) &
                Builders<StatisticsDbModel>.Filter.Eq("BoardId", boardId) &
                Builders<StatisticsDbModel>.Filter.Eq("Type", "connection");
            var results = await statisticsCollection
                .Find(filter)
                .Project(Builders<StatisticsDbModel>.Projection
                    .Include("Time")
                    .Include("Link")
                    .Include("ResponseStatus")
                    .Include("Exception")
                    .Exclude("_id"))
                .Sort(new BsonDocument { { "Time", -1 } })
                .Limit(100)
                .ToListAsync();

            var connectionStatsList = new List<StatisticsConnectionModel>();
            foreach (var item in results)
            {
                connectionStatsList.Add(BsonSerializer.Deserialize<StatisticsConnectionModel>(item));
            }

            return connectionStatsList;
        }

        /// <summary>
        /// Gets all data about a single presentation.
        /// </summary>
        /// <param name="id">The id of the presentation whose data will be obtained.</param>
        /// <returns>An object containing all data stored in the database about the specified presentation.</returns>
        public Task<BoardPresentationDbModel> GetPresentation(string id)
        {
            var filter = Builders<BoardPresentationDbModel>.Filter.Eq("_id", id);
            var document = presentationCollection.Find(filter).FirstOrDefaultAsync();  //returns null if no match
            return document;
        }

        /// <summary>
        /// Deletes the specified presentation.
        /// </summary>
        /// <param name="id">The id of the presentation to be deleted.</param>
        public Task DeletePresentation(string id)
        {
            var filter = Builders<BoardPresentationDbModel>.Filter.Eq("Id", id);
            return presentationCollection.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Generates an auto-incremented id for new presentations.
        /// </summary>
        /// <returns>The id for a new presentation.</returns>
        private int GeneratePresentationId()
        {
            var id = (int)presentationCollection.Count(new BsonDocument());
            return ++id;
        }
    }
}