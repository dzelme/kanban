﻿using Xunit;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ESL.CO.React.JiraIntegration;
using System.Collections.Generic;
using System.Linq;


namespace ESL.CO.Tests
{
    public class BoardCreatorTest
    {
        private Mock<IMemoryCache> memoryCache;
        private Mock<IJiraClient> jiraClient;
        private BoardCreator controller;
        private BoardConfig boardConfiguration;
        private Board cachedBoard;
        private Board testBoard;
        private IssueList issuePageOne;
        private IssueList issuePageTwo;
        private IssueList issueOnlyPage;

        public BoardCreatorTest()
        {
            memoryCache = new Mock<IMemoryCache>();
            jiraClient = new Mock<IJiraClient>();

            boardConfiguration = new BoardConfig()
            {
                Id = 74,
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

            cachedBoard = new Board(74);
            testBoard = new Board(80);

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

            issueOnlyPage = new IssueList()
            {
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

            controller = new BoardCreator(jiraClient.Object);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_From_Cache_If_Jira_Is_Not_Available()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult<BoardConfig>(null));

            object board = cachedBoard;
            memoryCache.Setup(s=>s.TryGetValue(74, out board)).Returns(true);

            // Act
            var actual = controller.CreateBoardModel(74, memoryCache.Object).Result;

            // Assert
            Assert.True(actual.FromCache);
            Assert.Equal(74,actual.Id);
        }

        [Fact]
        public void CreateBoardModel_Should_Create_New_Board_Because_Jira_Not_Aviable_And_Cache_Empty()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult<BoardConfig>(null));

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(80, out board)).Returns(false);

            // Act
            var actual = controller.CreateBoardModel(80, memoryCache.Object).Result;

            // Assert
            Assert.Equal(testBoard, actual);
            Assert.False(actual.FromCache);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrive_Board_Config_From_Jira_But_No_Issues_After_So_Retreive_From_Cache()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("board/74/issue", 74)).Returns(Task.FromResult<IssueList>(null));

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(74, out board)).Returns(true);

            // Act
            var actual = controller.CreateBoardModel(74, memoryCache.Object).Result;

            // Assert
            Assert.True(actual.FromCache);
            Assert.Equal(74,actual.Id);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrive_Board_Config_From_Jira_But_No_Issues_After_And_Cache_Is_Empty_So_Create_New_Board()
        {
            // Arrange
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("board/74/issue", 74)).Returns(Task.FromResult<IssueList>(null));

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(80, out board)).Returns(false);

            // Act
            var actual = controller.CreateBoardModel(80, memoryCache.Object).Result;

            // Assert
            Assert.False(actual.FromCache);
            Assert.Equal(testBoard, actual);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_From_Jira_And_Issues_From_One_Page()
        {
            // Arrange

            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult(boardConfiguration));
            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>("board/74/issue", 74)).Returns(Task.FromResult(issueOnlyPage));

            // Act
            var actual = controller.CreateBoardModel(74, memoryCache.Object).Result;

            // Assert

            Assert.Equal("Test Board", actual.Name);
            Assert.Equal(74, actual.Id);
            Assert.Equal(3, actual.Columns.Count());
            Assert.Single(actual.Rows);
            Assert.Single(actual.Columns[0].Issues);
            Assert.Single(actual.Columns[1].Issues);
            Assert.Empty(actual.Columns[2].Issues);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_From_Jira_And_Issues_From_Multiple_Page()
        {
            // Arrange
         
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult(boardConfiguration));

            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>(It.IsAny<string>(), 74)).Returns((string a, int i) =>
            {
                switch (a)
                {
                    case "board/74/issue": return Task.FromResult(issuePageOne);
                    case "board/74/issue?startAt=2": return Task.FromResult(issuePageTwo);
                    default: return Task.FromResult<IssueList>(null);
                }
            });

            // Act
            var actual = controller.CreateBoardModel(74, memoryCache.Object).Result;

            // Assert

            Assert.Equal("Test Board", actual.Name);
            Assert.Equal(74, actual.Id);
            Assert.Equal(3, actual.Columns.Count());
            Assert.Equal(2, actual.Rows.Count());
            Assert.Equal(2, actual.Columns[0].Issues.Count());
            Assert.Single(actual.Columns[1].Issues);
            Assert.Single(actual.Columns[2].Issues);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_From_Jira_And_Issues_From_First_Page_But_Not_From_Second_And_Cache_Empty_So_Create_New_Board()
        {
            // Arrange
         
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult(boardConfiguration));

            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>(It.IsAny<string>(), 74)).Returns((string a, int i) =>
            {
                switch (a)
                {
                    case "board/74/issue": return Task.FromResult(issuePageOne);
                    case "board/74/issue?startAt=2": return Task.FromResult<IssueList>(null);
                    default: return Task.FromResult<IssueList>(null);
                }
            });

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(80, out board)).Returns(false);

            // Act
            var actual = controller.CreateBoardModel(80, memoryCache.Object).Result;

            // Assert

            Assert.Equal(testBoard, actual);
            Assert.False(actual.FromCache);
        }

        [Fact]
        public void CreateBoardModel_Should_Retrieve_Board_Config_From_Jira_And_Issues_From_First_Page_But_Not_From_Second_So_Load_From_Cache()
        {
            // Arrange
           
            jiraClient.Setup(a => a.GetBoardDataAsync<BoardConfig>("board/74/configuration", 74)).Returns(Task.FromResult(boardConfiguration));

            jiraClient.Setup(a => a.GetBoardDataAsync<IssueList>(It.IsAny<string>(), 74)).Returns((string a, int i) =>
            {
                switch (a)
                {
                    case "board/74/issue": return Task.FromResult(issuePageOne);
                    case "board/74/issue?startAt=2": return Task.FromResult<IssueList>(null);
                    default: return Task.FromResult<IssueList>(null);
                }
            });

            object board = cachedBoard;
            memoryCache.Setup(s => s.TryGetValue(74, out board)).Returns(true);

            // Act
            var actual = controller.CreateBoardModel(74, memoryCache.Object).Result;

            // Assert
            Assert.True(actual.FromCache);
            Assert.Equal(74,actual.Id);
        }
    }
}