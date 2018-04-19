using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ESL.CO.React.JiraIntegration;
using System.Collections.Generic;
using System.Linq;

namespace ESL.CO.Tests
{
    public class BoardCreatorTests
    {
        private Mock<IMemoryCache> memoryCache;
        private Mock<IJiraClient> jiraClient;
        private BoardCreator controller;
        private BoardConfig boardConfiguration;
        private ColorList colorList;
        private Board cachedBoard;
        private IssueList issuePageOne;
        private IssueList issuePageTwo;
        private Credentials credentials;
        private string presentationId;

        public BoardCreatorTests()
        {
            memoryCache = new Mock<IMemoryCache>();
            jiraClient = new Mock<IJiraClient>();
            credentials = new Credentials();
            presentationId = "1";

            boardConfiguration = new BoardConfig()
            {
                Id = "74",
                Name = "Test Board",

                ColumnConfig = new ColumnConfig()
                {
                    Columns = new List<Column>()
                    {
                        new Column()
                        {
                            Name = "To-Do",
                            Statuses = new List<ColumnStatus>
                            {
                               new ColumnStatus()
                               {
                                   Id = "1"
                               }
                            }
                        },
                        new Column()
                        {
                            Name = "In Progress",
                            Statuses = new List<ColumnStatus>
                            {
                               new ColumnStatus()
                               {
                                   Id = "2"
                               }
                            }
                        },
                        new Column()
                        {
                            Name = "Done",
                            Statuses = new List<ColumnStatus>
                            {
                               new ColumnStatus()
                               {
                                   Id = "3"
                               }
                            }
                        }
                    }
                }
            };

            colorList = new ColorList()
            {
                CardColors = new List<CardColor>{
                    new CardColor()
                    {
                         Color = "#ff0000",
                         DisplayValue = "Blocker",
                         Value = "1"
                    },
                    new CardColor()
                    {
                         Color = "#ffc0c0",
                         DisplayValue = "Critical",
                         Value = "2"
                    },
                   new CardColor()
                    {
                         Color = "#c2ffc2",
                         DisplayValue = "Major",
                         Value = "3"
                    },
                    new CardColor()
                    {
                         Color = "#ffffc2",
                         DisplayValue = "Minor",
                         Value = "4"
                    },
                     new CardColor()
                    {
                          Color = "#bfbfbf",
                         DisplayValue = "Trivial" ,
                         Value = "5"
                    }
                }
            };

            cachedBoard = new Board("74");

            issuePageOne = new IssueList()
            {
                StartAt = 0,
                MaxResults = 2,
                Total = 4,
                Issues = new List<Issue>()
                {
                    new Issue()
                    {
                         Key = "TestIssue-1",
                         Fields = new Fields()
                         {
                             Assignee = new Assignee()
                             {
                                 DisplayName = "TestAssignee1"
                             },
                             Summary = "Hello World!",
                             Priority = new Priority()
                             {
                                 Name="Critical"
                             },
                             Status = new Status()
                             {
                                 Name = "To-Do",
                                 Id = "1"
                             }
                         }
                    },
                    new Issue()
                    {
                         Key = "TestIssue-2",
                         Fields = new Fields()
                         {
                             Assignee = new Assignee()
                             {
                                 DisplayName = "TestAssignee1"
                             },
                             Summary = "Hello World!",
                             Priority = new Priority()
                             {
                                 Name="Critical"
                             },
                             Status = new Status()
                             {
                                 Name = "In Progress",
                                 Id = "2"
                             }
                         }
                    },
                }
            };

            issuePageTwo = new IssueList()
            {
                StartAt = 2,
                MaxResults = 2,
                Total = 4,
                Issues = new List<Issue>()
                {
                    new Issue()
                    {
                         Key = "TestIssue-3",
                         Fields = new Fields()
                         {
                             Assignee = new Assignee()
                             {
                                 DisplayName = "TestAssignee2"
                             },
                             Summary = "Hello World 2!",
                             Priority = new Priority()
                             {
                                 Name="Trivial"
                             },
                             Status = new Status()
                             {
                                 Name = "To-Do",
                                 Id = "1"
                             }
                         }
                    },
                    new Issue()
                    {
                         Key = "TestIssue-4",
                         Fields = new Fields()
                         {
                             Assignee = new Assignee()
                             {
                                 DisplayName = "TestAssignee2"
                             },
                             Summary = "Hello World 2!",
                             Priority = new Priority()
                             {
                                 Name="Major"
                             },
                             Status = new Status()
                             {
                                 Name = "Done",
                                 Id = "3"
                             }
                         }
                    },
                }
            };

            controller = new BoardCreator(jiraClient.Object);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_The_Board_From_Cache_If_BoardConfig_From_Jira_Is_Not_Available()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult<BoardConfig>(null)).Verifiable();
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(true).Verifiable();

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.Verify();
            memoryCache.Verify();
            Assert.NotNull(actual);
            Assert.True(actual.FromCache);
            Assert.Equal("74", actual.Id);
        }

        [Fact]
        public void CreateBoardModel_Should_Create_A_New_Board_Because_BoardConfig_From_Jira_Not_Available_And_Cache_Empty()
        {
            // Arrange
            var testBoard = new Board("80");
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult<BoardConfig>(null));
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("80", out board)).Returns(false).Verifiable();

            // Act
            var actual = controller.CreateBoardModel("80", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.Verify();
            memoryCache.Verify();
            Assert.Equal(testBoard, actual);
            Assert.False(actual.FromCache);
        }

        [Fact]
        public void CreateBoardModel_Should_Return_A_Board_With_Empty_ColorList_If_ColorList_From_Jira_Not_Available()
        {
            // Arrange
            var issuePageOne = this.issuePageOne;
            issuePageOne.StartAt = issuePageOne.MaxResults = issuePageOne.Total = 0;
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult<ColorList>(null));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("agile/1.0/board/74/issue", credentials, "74", presentationId)).Returns(Task.FromResult(issuePageOne)).Verifiable();
            
            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.VerifyAll();
            Assert.NotNull(actual);
            Assert.Equal("74", actual.Id);
            Assert.Equal("Test Board", actual.Name);
            Assert.Equal(3, actual.Columns.Count());
            Assert.Single(actual.Rows);
            Assert.Single(actual.Columns[0].Issues);
            Assert.Single(actual.Columns[1].Issues);
            Assert.Empty(actual.Columns[2].Issues);
        }

        [Fact]
        public void CreateBoardModel_Should_Return_The_Board_From_Cache_If_IssueList_From_Jira_Not_Available()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult(colorList));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("agile/1.0/board/74/issue", credentials, "74", presentationId)).Returns(Task.FromResult<IssueList>(null));
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(true);

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.VerifyAll();
            Assert.True(actual.FromCache);
            Assert.Equal("74", actual.Id);
        }

        [Fact]
        public void CreateBoardModel_Should_Return_An_Issueless_Board_If_IssueList_From_Jira_Not_Available_And_The_Board_Is_Not_In_Cache()
        {
            // Arrange
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(false).Verifiable();
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult(colorList));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("agile/1.0/board/74/issue", credentials, "74", presentationId)).Returns(Task.FromResult<IssueList>(null));

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            memoryCache.Verify();
            jiraClient.VerifyAll();
            Assert.NotNull(actual);
            Assert.Equal("74", actual.Id);
            Assert.Equal("Test Board", actual.Name);  // config
            Assert.Equal(5, actual.CardColors.Count);  // color list
            Assert.Equal("#ff0000", actual.CardColors[0].Color);
            Assert.Equal("#bfbfbf", actual.CardColors[4].Color);
            Assert.Empty(actual.Columns);  // issues
            Assert.False(actual.FromCache);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_And_Color_List_And_One_Page_Of_Issues_From_Jira()
        {
            // Arrange
            var issuePageOne = this.issuePageOne;
            issuePageOne.StartAt = issuePageOne.MaxResults = issuePageOne.Total = 0;
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult(colorList));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("agile/1.0/board/74/issue", credentials, "74", presentationId)).Returns(Task.FromResult(issuePageOne));

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.VerifyAll();
            Assert.NotNull(actual);
            Assert.Equal("74", actual.Id);
            Assert.Equal("Test Board", actual.Name);
            Assert.Equal(3, actual.Columns.Count());
            Assert.Single(actual.Rows);
            Assert.Single(actual.Columns[0].Issues);
            Assert.Single(actual.Columns[1].Issues);
            Assert.Empty(actual.Columns[2].Issues);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_And_Color_List_And_Multiple_Pages_Of_Issues_From_Jira()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult(colorList));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>(It.IsAny<string>(), credentials, "74", presentationId)).Returns((string a, Credentials b, string i, string p) =>
            {
                switch (a)
                {
                    case "agile/1.0/board/74/issue": return Task.FromResult(issuePageOne);
                    case "agile/1.0/board/74/issue?startAt=2": return Task.FromResult(issuePageTwo);
                    default: return Task.FromResult<IssueList>(null);
                }
            });

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.VerifyAll();
            Assert.Equal("Test Board", actual.Name);
            Assert.Equal("74", actual.Id);
            Assert.Equal(3, actual.Columns.Count());
            Assert.Equal(2, actual.Rows.Count());
            Assert.Equal(2, actual.Columns[0].Issues.Count());
            Assert.Single(actual.Columns[1].Issues);
            Assert.Single(actual.Columns[2].Issues);
        }

        [Fact]
        public void CreateBoardModel_Should_Return_An_Incomplete_Board_If_Second_Page_Of_Issues_Not_Received_From_Jira_And_The_Board_Is_Not_In_Cache()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult(colorList));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>(It.IsAny<string>(), credentials, "74", presentationId)).Returns((string a, Credentials b, string i, string p) =>
            {
                switch (a)
                {
                    case "agile/1.0/board/74/issue": return Task.FromResult(issuePageOne);
                    case "agile/1.0/board/74/issue?startAt=2": return Task.FromResult<IssueList>(null);
                    default: return Task.FromResult<IssueList>(null);
                }
            });
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(false).Verifiable();

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.VerifyAll();
            memoryCache.Verify();
            Assert.NotNull(actual);
            Assert.Equal("74", actual.Id);
            Assert.Equal("Test Board", actual.Name);  // config
            Assert.Equal(5, actual.CardColors.Count);  // color list
            Assert.Equal("#ff0000", actual.CardColors[0].Color);
            Assert.Equal("#bfbfbf", actual.CardColors[4].Color);
            Assert.Equal(3, actual.Columns.Count);  // issues
            Assert.Equal("To-Do", actual.Columns[0].Name);
            Assert.Equal("Done", actual.Columns[2].Name);
            Assert.Single(actual.Rows);
            Assert.Single(actual.Columns[0].Issues);
            Assert.Single(actual.Columns[1].Issues);
            Assert.Empty(actual.Columns[2].Issues);
            Assert.False(actual.FromCache);
        }

        [Fact]
        public void CreateBoardModel_Should_Return_The_Board_From_Cache_If_The_Second_Page_Of_Issues_Not_Received_From_Jira()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("agile/1.0/board/74/configuration", credentials, "74", presentationId)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<ColorList>("greenhopper/1.0/cardcolors/74/strategy/priority", credentials, "74", presentationId)).Returns(Task.FromResult(colorList));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>(It.IsAny<string>(), credentials, "74", presentationId)).Returns((string a, Credentials b, string i, string p) =>
            {
                switch (a)
                {
                    case "agile/1.0/board/74/issue": return Task.FromResult(issuePageOne);
                    default: return Task.FromResult<IssueList>(null);
                }
            });
            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue("74", out board)).Returns(true);

            // Act
            var actual = controller.CreateBoardModel("74", presentationId, credentials, memoryCache.Object).Result;

            // Assert
            jiraClient.VerifyAll();
            Assert.True(actual.FromCache);
            Assert.Equal("74", actual.Id);
        }
    }
}
