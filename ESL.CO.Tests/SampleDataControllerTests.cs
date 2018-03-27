using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ESL.CO.React.JiraIntegration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ESL.CO.Tests
{
    public class SampleDataControllerTests
    {
        private Mock<IMemoryCache> memoryCache;
        private Mock<IAppSettings> appSettings;
        private Mock<IJiraClient> jiraClient;
        private Mock<IBoardCreator> boardCreator;
        private FullBoardList cachedSettings;
        private Board cachedBoard;
        private Board testBoard1;
        private Board testBoard2;
        private SampleDataController controller;
        private IOptions<Paths> paths;
        private Credentials credentials;
        private string credentialsString;

        public SampleDataControllerTests()
        {
            memoryCache = new Mock<IMemoryCache>();
            appSettings = new Mock<IAppSettings>();
            jiraClient = new Mock<IJiraClient>();
            boardCreator = new Mock<IBoardCreator>();
            credentials = new Credentials { Username = "", Password = "" };
            credentialsString = credentials.Username + ":" + credentials.Password;

            paths = Options.Create(new Paths
            {
                LogDirectoryPath = ".\\data\\logs\\",
                PresentationDirectoryPath = ".\\data\\presentations\\"
            });

            cachedSettings = new FullBoardList
            {
                Values = new List<Value>()
                {
                    new Value { Id = 74 },
                    new Value { Id = 75 },
                    new Value { Id = 76 },
                    new Value { Id = 77 },
                }
            };

            cachedBoard = new Board(74);

            testBoard1 = new Board(74);
            testBoard2 = new Board(80);

            appSettings.Setup(a => a.GetSavedAppSettings()).Returns(cachedSettings);
            controller = new SampleDataController(memoryCache.Object, jiraClient.Object, appSettings.Object, boardCreator.Object, paths);
        }

        [Fact]
        public void BoardList_Should_Retrieve_Values_From_Cache_If_Jira_Is_Not_Available()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>("board/", credentialsString, 0)).Returns(Task.FromResult<BoardList>(null));

            // Act
            var actual = controller.BoardList(credentials).Result;

            // Assert
            Assert.Equal(cachedSettings.Values, actual);
        }

        [Fact]
        public void BoardList_Should_Retrieve_Values_From_Jira()
        {
            // Arrange
            var boardList = new BoardList()
            {
                IsLast = true,
                Values = new List<Value>() {
                    new Value {  Id = 74 },
                    new Value {  Id = 75 },
                }
            };

            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>("board/", credentialsString, 0)).Returns(Task.FromResult(boardList));

            // Act
            var actual = controller.BoardList(credentials).Result;

            // Assert
            Assert.Equal(2, actual.Count());
            Assert.Contains(actual, x => x.Id == 74);
            Assert.Contains(actual, x => x.Id == 75);
        }

        [Fact]
        public void BoardList_Should_Retrieve_Values_From_Jira_Multiple_Pages()
        {
            // Arrange
            var firstPage = new BoardList()
            {
                IsLast = false,
                Values = new List<Value>() {
                    new Value {  Id = 74 },
                    new Value {  Id = 75 },
                },
                StartAt = 0,
                MaxResults = 2
            };
            var secondPage = new BoardList()
            {
                IsLast = true,
                Values = new List<Value>() {
                    new Value {  Id = 76 },
                    new Value {  Id = 77 },
                },
                StartAt = 2,
                MaxResults = 2
            };

            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>(It.IsAny<string>(), credentialsString, 0)).Returns((string a, string b, int i) =>
            {
                switch (a)
                {
                    case "board/": return Task.FromResult(firstPage);
                    case "board?startAt=2": return Task.FromResult(secondPage);
                    default: return Task.FromResult<BoardList>(null);
                }
            });

            // Act
            var actual = controller.BoardList(credentials).Result;

            // Assert
            Assert.Equal(4, actual.Count());
            Assert.Contains(actual, x => x.Id == 74);
            Assert.Contains(actual, x => x.Id == 75);
            Assert.Contains(actual, x => x.Id == 76);
            Assert.Contains(actual, x => x.Id == 77);
        }

        [Fact]
        public void BoardList_Should_Retrieve_Values_From_Jira_First_Page_But_Not_From_Second()
        {
            // Arrange
            var firstPage = new BoardList()
            {
                IsLast = false,
                Values = new List<Value>() {
                    new Value {  Id = 74 },
                    new Value {  Id = 75 },
                },
                StartAt = 0,
                MaxResults = 2
            };

            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>(It.IsAny<string>(), credentialsString, 0)).Returns((string a, string b, int i) =>
            {
                switch (a)
                {
                    case "board/": return Task.FromResult(firstPage);
                    case "board?startAt=2": return Task.FromResult<BoardList>(null);
                    default: return Task.FromResult<BoardList>(null);
                }
            });

            // Act
            var actual = controller.BoardList(credentials).Result;

            // Assert
            Assert.Equal(2, actual.Count());
            Assert.NotEqual(cachedSettings.Values.Count(), actual.Count());
            Assert.Contains(actual, x => x.Id == 74);
            Assert.Contains(actual, x => x.Id == 75);
            Assert.DoesNotContain(actual, x => x.Id == 76);
            Assert.DoesNotContain(actual, x => x.Id == 77);
        }

        [Fact]
        public void BoardData_Should_Return_Board_With_HasChanged_True()
        {
            // Arrange
            boardCreator.Setup(a => a.CreateBoardModel(74, credentialsString, memoryCache.Object)).Returns(Task.FromResult(testBoard1));

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(74, out board)).Returns(false);

            // Act
            var actual = controller.BoardData(74,credentials).Result;

            // Assert
            Assert.True(actual.HasChanged);
        }

        [Fact]
        public void BoardData_Should_Return_Board_With_HasChanged_False()
        {
            // Arrange
            boardCreator.Setup(a => a.CreateBoardModel(74, credentialsString, memoryCache.Object)).Returns(Task.FromResult(testBoard1));

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(74, out board)).Returns(true);

            // Act
            var actual = controller.BoardData(74,credentials).Result;

            // Assert
            Assert.False(actual.HasChanged);
        }

        [Fact]
        public void NeedsRedraw_Should_Return_False()
        {
             // Arrange
             object board = cachedBoard;
             memoryCache.Setup(s => s.TryGetValue(74, out board)).Returns(true);
          
            // Act
            var actual = controller.NeedsRedraw(testBoard1);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void NeedsRedraw_Should_Return_True()
        {
            // Arrange
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(80, out board)).Returns(false);

            // Act
            var actual = controller.NeedsRedraw(testBoard2);

            // Assert
            Assert.True(actual);
        }
    }
}

