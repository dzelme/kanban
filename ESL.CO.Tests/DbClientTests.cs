using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.DbConnection;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;

namespace ESL.CO.Tests
{
    public class DbClientTests
    {
        private Mock<DbClient> dbClient;
        private IOptions<DbSettings> dbSettings;

        private IMongoCollection<StatisticsDbModel> statisticsCollection;
        private IMongoCollection<BoardPresentationDbModel> presentationCollection;

        public DbClientTests()
        {
            dbSettings = Options.Create(new DbSettings
            {
                MongoDbUrl = "",
                DatabaseName = "",
                StatisticsCollectionName = "",
                PresentationsCollectionName = "",
                StatisticsEntryTtlHours = 168
            });


            //var message = string.Empty;

            //var serverSettings = new MongoServerSettings()
            //{
            //    GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard,
            //    ReadEncoding = new UTF8Encoding(),
            //    ReadPreference = new ReadPreference(),
            //    WriteConcern = new WriteConcern(),
            //    WriteEncoding = new UTF8Encoding()
            //};

            //var server = new Mock<MongoServer>(serverSettings);
            //server.Setup(s => s.Settings).Returns(serverSettings);
            //server.Setup(s => s.IsDatabaseNameValid(It.IsAny<string>(), out message)).Returns(true);

            //var databaseSettings = new MongoDatabaseSettings()
            //{
            //    GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard,
            //    ReadEncoding = new UTF8Encoding(),
            //    ReadPreference = new ReadPreference(),
            //    WriteConcern = new WriteConcern(),
            //    WriteEncoding = new UTF8Encoding()
            //};

            //var database = new Mock<MongoDatabase>(server.Object, "test", databaseSettings);
            //database.Setup(db => db.Settings).Returns(databaseSettings);
            //database.Setup(db => db.IsCollectionNameValid(It.IsAny<string>(), out message)).Returns(true);

            //var mockedCollection = collection.Object;


            dbClient = new Mock<DbClient>(dbSettings);
        }

        [Fact]
        public void SaveStatisticsAsync_Should_Return_xxxx()
        {

        }

    }
}

