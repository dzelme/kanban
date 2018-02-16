using ESL.CO.React.JiraIntegration;
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
            var xxx = client.GetIssueListAsync("board/" + "620" + "/issue").Result;
            Assert.NotNull(xxx);
        }
    }
}
