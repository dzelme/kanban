using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.DbConnection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace ESL.CO.Tests
{
    public class SampleDataControllerTests
    {
        private Mock<IMemoryCache> memoryCache;
        private Mock<IJiraClient> jiraClient;
        private Mock<IBoardCreator> boardCreator;
        private Mock<IDbClient> dbClient;
        private IOptions<UserSettings> userSettings;
        private Board cachedBoard;
        private Board testBoard1;
        private Board testBoard2;
        private SampleDataController controller;
        private Credentials credentials;
        private string credentialsString;
        private string presentationId;
        private BoardPresentationDbModel presentationDbModel;

        public SampleDataControllerTests()
        {
            jiraClient = new Mock<IJiraClient>();
            memoryCache = new Mock<IMemoryCache>();
            boardCreator = new Mock<IBoardCreator>();
            dbClient = new Mock<IDbClient>();
            credentials = new Credentials { Username = "", Password = "" };
            credentialsString = credentials.Username + ":" + credentials.Password;
            presentationId = "1";

            presentationDbModel = new BoardPresentationDbModel
            {
                Id = presentationId,
                Credentials = credentials,
            };

            userSettings = Options.Create(new UserSettings
            {
                RefreshRateMax = 100,
                RefreshRateMin = 1,
                TimeShownMax = 100,
                TimeShownMin = 1
            });

            cachedBoard = new Board("74");

            testBoard1 = new Board("74");
            testBoard2 = new Board("80");

            controller = new SampleDataController(memoryCache.Object, jiraClient.Object, boardCreator.Object, dbClient.Object);
        }

        [Fact]
        public void BoardData_Should_Return_A_Board_With_Id_And_Flags_Only_If_Presentation_Not_Obtained_From_Database_And_Hence_Credentials_Are_Unknown()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation(presentationId)).Returns(Task.FromResult<BoardPresentationDbModel>(null)).Verifiable();
            boardCreator.Setup(a => a.CreateBoardModel("74", presentationId, It.IsAny<Credentials>(), memoryCache.Object)).Returns(Task.FromResult(testBoard1)).Verifiable();
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(false).Verifiable();

            // Act
            var actual = controller.BoardData("74", presentationId).Result;
            
            // Assert
            dbClient.Verify();
            boardCreator.Verify();
            memoryCache.Verify();
            Assert.NotNull(actual);
            Assert.Equal("74", actual.Id);
            Assert.Empty(actual.Name);
            Assert.Empty(actual.Columns);
            Assert.Empty(actual.Rows);
            Assert.Empty(actual.CardColors);
            Assert.True(actual.HasChanged);
        }

        [Fact]
        public void BoardData_Should_Return_A_Board_With_HasChanged_True_If_The_Board_Is_Not_In_Cache()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation(presentationId)).Returns(Task.FromResult(presentationDbModel)).Verifiable();
            boardCreator.Setup(a => a.CreateBoardModel("74", presentationId, It.IsAny<Credentials>(), memoryCache.Object)).Returns(Task.FromResult(testBoard1)).Verifiable();
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(false).Verifiable();

            // Act
            var actual = controller.BoardData("74", presentationId).Result;

            // Assert
            dbClient.Verify();
            boardCreator.Verify();
            memoryCache.Verify();
            Assert.NotNull(actual);
            Assert.True(actual.HasChanged);
        }

        [Fact]
        public void BoardData_Should_Return_A_Board_With_HasChanged_False_If_The_Board_Is_Unchanged_And_In_Cache()
        {
            // Arrange
            dbClient.Setup(a => a.GetPresentation(presentationId)).Returns(Task.FromResult(presentationDbModel)).Verifiable();
            boardCreator.Setup(a => a.CreateBoardModel("74", presentationId, It.IsAny<Credentials>(), memoryCache.Object)).Returns(Task.FromResult(testBoard1)).Verifiable();
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(true).Verifiable();

            // Act
            var actual = controller.BoardData("74", presentationId).Result;

            // Assert
            dbClient.Verify();
            boardCreator.Verify();
            memoryCache.Verify();
            Assert.NotNull(actual);
            Assert.False(actual.HasChanged);
        }

        [Fact]
        public void NeedsRedraw_Should_Return_False_If_Board_Is_Unchanged_And_In_Cache()
        {
             // Arrange
             object board = cachedBoard;
             memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(true).Verifiable();
          
            // Act
            var actual = controller.NeedsRedraw(testBoard1);

            // Assert
            memoryCache.Verify();
            Assert.False(actual);
        }

        [Fact]
        public void NeedsRedraw_Should_Return_True_If_Board_Is_Different_From_The_One_In_Cache()
        {
            // Arrange
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(true).Verifiable();
            testBoard1.Name = "difference";

            // Act
            var actual = controller.NeedsRedraw(testBoard1);

            // Assert
            memoryCache.Verify();
            Assert.True(actual);
        }

        [Fact]
        public void NeedsRedraw_Should_Return_True_If_Board_Is_Not_In_Cache()
        {
            // Arrange
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("80", out board)).Returns(false).Verifiable();

            // Act
            var actual = controller.NeedsRedraw(testBoard2);

            // Assert
            memoryCache.Verify();
            Assert.True(actual);
        }
    }
}

