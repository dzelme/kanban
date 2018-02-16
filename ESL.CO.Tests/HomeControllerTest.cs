using ESL.CO.React.Controllers;
using System;
using Xunit;

namespace ESL.CO.Tests
{
    public class HomeControllerTest
    {
        [Fact]
        public void Test1()
        {
            var controller = new HomeController();
            var result = controller.Index().Result;
            Assert.NotNull(result);
        }
    }
}
