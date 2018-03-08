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
                AllValues = new List<Value>()
                {
                    new Value { Id = 74 }
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
            Assert.Equal(cachedSettings.AllValues, actual);
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
    }
}
