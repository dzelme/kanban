using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Controllers;
using ESL.CO.React.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ESL.CO.React.JiraIntegration;

namespace ESL.CO.Tests
{
    public class SampleDataControllerTests
    {
        [Fact]
        public void BoardList_Should_Retrieve_Values_From_Jira()
        {
            var memoryCache = new Mock<IMemoryCache>();
            var appSettings = new Mock<IAppSettings>();
            var jiraClient = new Mock<IJiraClient>();
            var controller = new SampleDataController(memoryCache.Object, jiraClient.Object, appSettings.Object);
            var actual = controller.BoardList().Result;
            

        }
    }
}
