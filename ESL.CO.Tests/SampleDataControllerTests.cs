using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ESL.CO.React.JiraIntegration;
using System.Collections.Generic;
using System.Linq;

namespace ESL.CO.Tests
{
    public class SampleDataControllerTests
    {
        private Mock<IMemoryCache> memoryCache;
        private Mock<IAppSettings> appSettings;
        private Mock<IJiraClient> jiraClient;
        private Mock<IBoardCreator> boardCreator;
        private FullBoardList cachedSettings;
        private Board cachedboardSettings;
        private SampleDataController controller;

        public SampleDataControllerTests()
        {
            memoryCache = new Mock<IMemoryCache>();
            appSettings = new Mock<IAppSettings>();
            jiraClient = new Mock<IJiraClient>();
            boardCreator = new Mock<IBoardCreator>();

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

            cachedboardSettings = new Board()
            {
                Id = 74
            };

            appSettings.Setup(a => a.GetSavedAppSettings()).Returns(cachedSettings);
            controller = new SampleDataController(memoryCache.Object, jiraClient.Object, appSettings.Object, boardCreator.Object);
        }

        [Fact]
        public void BoardList_Should_Retrieve_Values_From_Cache_If_Jira_Is_Not_Available()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>("board/", 0)).Returns(Task.FromResult<BoardList>(null));

            // Act
            var actual = controller.BoardList().Result;

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

            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>("board/", 0)).Returns(Task.FromResult(boardList));

            // Act
            var actual = controller.BoardList().Result;

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

            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>(It.IsAny<string>(), 0)).Returns((string a, int i) =>
            {
                switch (a)
                {
                    case "board/": return Task.FromResult(firstPage);
                    case "board?startAt=2": return Task.FromResult(secondPage);
                    default: return Task.FromResult<BoardList>(null);
                }
            });

            // Act
            var actual = controller.BoardList().Result;

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


            jiraClient.Setup(a => a.GetBoardDataAsync<BoardList>(It.IsAny<string>(), 0)).Returns((string a, int i) =>
            {
                switch (a)
                {
                    case "board/": return Task.FromResult(firstPage);
                    case "board?startAt=2": return Task.FromResult<BoardList>(null);
                    default: return Task.FromResult<BoardList>(null);
                }
            });

            // Act
            var actual = controller.BoardList().Result;

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
            var board = new Board()
            {
                Id = 74,
                HasChanged = false
            };

            boardCreator.Setup(a => a.CreateBoardModel(74, memoryCache.Object)).Returns(Task.FromResult(board));

            controller.NeedsRedraw(board).Equals(true);

            // Act
            var actual = controller.BoardData(74).Result;

            // Assert
            Assert.True(actual.HasChanged);
        }

        /// <summary>
        /// Tests nenostrādā, hasChanged ir true, nevis false
        /// </summary>
        [Fact]
        public void BoardData_Should_Return_Board_With_HasChanged_False()
        {
            // Arrange
            var board = new Board()
            {
                Id = 74,
                HasChanged = false
            };

            boardCreator.Setup(a => a.CreateBoardModel(74, memoryCache.Object)).Returns(Task.FromResult(board));

            controller.NeedsRedraw(board).Equals(false);

            // Act
            var actual = controller.BoardData(74).Result;

            // Assert
            Assert.False(actual.HasChanged);
        }


        /// <summary>
        /// Tests nenostrādā, atgriež true nevis false
        /// </summary>
        [Fact]
        public void NeedsRedraw_Should_Return_False()
        {
            // Arrange
            var board = new Board()
            {
                Id = 74
            };

            board.Equals(cachedboardSettings).Equals(true);

            // Act
            var actual = controller.NeedsRedraw(board);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void NeedsRedraw_Should_Return_True()
        {
            // Arrange
            var board = new Board()
            {
                Id = 80
            };

            board.Equals(cachedboardSettings).Equals(false);

            // Act
            var actual = controller.NeedsRedraw(board);

            // Assert
            Assert.True(actual);
        }
    }
}

