using ESL.CO.JiraIntegration;
using System;
using Xunit;

namespace ESL.CO.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var client = new JiraClient();
            var xxx = client.GetIssueListAsync("").Result;
            Assert.NotNull(xxx);
        }
    }
}
