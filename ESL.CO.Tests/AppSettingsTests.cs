using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ESL.CO.React.JiraIntegration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace ESL.CO.Tests
{
    public class AppSettingsTests
    {
        private AppSettings controller;
        private FullBoardList listSaved;
        private FullBoardList listCurrent;
        private IOptions<Paths> paths;

        public AppSettingsTests()
        {
            paths = Options.Create(new Paths());
            controller = new AppSettings(paths);
        }

        ///<Summary>
        ///Testi MergeSettings, ja mainīs klasi uz ne-statisku
        ///</Summary>
        /*
        [Fact]
        public void MergeSettings_Should_Return_Null_Because_Both_Parameters_Null()
        {
            // Arrange
            // Act
            var actual = controller.MergeSettings(ListSaved,ListCurrent);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void MergeSettings_Should_Return_Current_BoardList_Because_There_Is_No_Saved_One()
        {
            // Arrange

            var ListSaved = new FullBoardList();
            var ListCurrent = new FullBoardList()
            {
                Values = new List<Value>()
                {
                    new Value { Id = 74 },
                    new Value { Id = 75 },
                    new Value { Id = 76 },
                    new Value { Id = 77 },
                }
            };

            // Act
            var actual = controller.MergeSettings(ListSaved, ListCurrent);

            // Assert
            Assert.Equal(ListCurrent,actual);
        }

        [Fact]
        public void MergeSettings_Should_Return_Saved_BoardList_Because_There_Is_No_Current_One()
        {
            // Arrange

            var ListSaved = new FullBoardList()
            {
                Values = new List<Value>()
                {
                    new Value { Id = 74 },
                    new Value { Id = 75 },
                    new Value { Id = 76 },
                    new Value { Id = 77 },
                }
            };

            var ListCurrent = new FullBoardList();

            // Act
            var actual = controller.MergeSettings(ListSaved, ListCurrent);

            // Assert
            Assert.Equal(ListSaved, actual);
        }*/
    }
}