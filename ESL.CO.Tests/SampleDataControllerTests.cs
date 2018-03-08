using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using Microsoft.AspNetCore.Mvc;
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
        private FullBoardList cachedSettings;
        private SampleDataController controller;

        public SampleDataControllerTests()
        {
           this.memoryCache = new Mock<IMemoryCache>();
           this.appSettings = new Mock<IAppSettings>();
           this.jiraClient = new Mock<IJiraClient>();

            this.cachedSettings = new FullBoardList
            {
                Values = new List<Value>()
                {
                    new Value { Id = 74 },
                    new Value { Id = 75 },
                    new Value { Id = 76 },
                    new Value { Id = 77 },
                }
            };
            appSettings.Setup(a => a.GetSavedAppSettings()).Returns(cachedSettings);
            this.controller = new SampleDataController(memoryCache.Object, jiraClient.Object, appSettings.Object);
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
            Assert.NotEqual(cachedSettings.AllValues, actual);
            Assert.Contains(actual, x => x.Id == 74);
            Assert.Contains(actual, x => x.Id == 75);
            Assert.DoesNotContain(actual, x => x.Id == 76);
            Assert.DoesNotContain(actual, x => x.Id == 77);
        }
        
    }
}
