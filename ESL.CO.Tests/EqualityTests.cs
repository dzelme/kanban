using System.Collections.Generic;
using Xunit;
using ESL.CO.React.Models;

namespace ESL.CO.Tests
{
    public class EqualityTests
    {
        [Theory]
        [InlineData(620)]
        [InlineData(0)]
        [InlineData(-6)]
        public void ReturnTrueGivenSameIds(int id)
        {
            //var id = 620;
            //IMemoryCache cache = default(IMemoryCache);

            var board1 = new Board(id); //creator.CreateBoardModel(id, cache).Result;
            var board2 = new Board(id); //creator.CreateBoardModel(id, cache).Result;

            Assert.True(board1.Equals(board2));
        }

        [Fact]
        public void ReturnFalseGivenDifferentIds()
        {
            var id1 = 620;
            var id2 = 943;

            var board1 = new Board(id1);
            var board2 = new Board(id2);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         

            Assert.False(board1.Equals(board2));
        }

        [Fact]
        public void ReturnFalseGivenNullObject()
        {
            var id = 620;

            //var creator = new BoardCreator();
            Board board1 = null;
            var board2 = new Board(id);

            Assert.False(board2.Equals(board1));
        }

        [Fact]
        public void ReturnFalseGivenNullPropertyObject()
        {
            var id = 620;

            //var creator = new BoardCreator();
            Board board1 = new Board
            {
                Id = id,
                Name = "kanban",
                FromCache = false,
                Message = "message",
                Columns = new List<BoardColumn>(),
                Rows = new List<BoardRow>()
            };
            var bc1 = new BoardColumn("bc");
            board1.Columns.Add(bc1);
            var issue1 = new Issue
            {
                Key = "123",
                Fields = new Fields
                {
                    Priority = new Priority
                    {
                        Id = "111",
                        Name = "big",
                    },
                    Assignee = null,  //difference
                    Status = new Status
                    {
                        Id = "111",
                        Name = "status",
                    },
                    Description = "description",
                    Summary = "summary",
                }
            };
            bc1.Issues.Add(issue1);


            Board board2 = new Board
            {
                Id = id,
                Name = "kanban",
                FromCache = false,
                Message = "message",
                Columns = new List<BoardColumn>(),
                Rows = new List<BoardRow>()
            };
            var bc2 = new BoardColumn("bc");
            board2.Columns.Add(bc2);
            var issue2 = new Issue
            {
                Key = "123",
                Fields = new Fields
                {
                    Priority = new Priority
                    {
                        Id = "111",
                        Name = "big",
                    },
                    Assignee = new Assignee
                    {
                        DisplayName = "displayName",  //difference
                    },
                    Status = new Status
                    {
                        Id = "111",
                        Name = "status",
                    },
                    Description = "description",
                    Summary = "summary",
                }
            };
            bc2.Issues.Add(issue2);
            
            Assert.False(board1.Equals(board2));
        }

        [Fact]
        public void ReturnTrueGivenEqualComplexObjects()
        {
            var id = 620;

            //var creator = new BoardCreator();
            Board board1 = new Board
            {
                Id = id,
                Name = "kanban",
                FromCache = false,
                Message = "message",
                Columns = new List<BoardColumn>(),
                Rows = new List<BoardRow>()
            };
            var bc1 = new BoardColumn("bc");
            board1.Columns.Add(bc1);
            var issue1 = new Issue
            {
                Key = "123",
                Fields = new Fields
                {
                    Priority = new Priority
                    {
                        Id = "111",
                        Name = "big",
                    },
                    Assignee = new Assignee
                    {
                        DisplayName = "displayName",
                    },
                    Status = new Status
                    {
                        Id = "111",
                        Name = "status",
                    },
                    Description = "description",
                    Summary = "summary",
                }
            };
            bc1.Issues.Add(issue1);


            Board board2 = new Board
            {
                Id = id,
                Name = "kanban",
                FromCache = false,
                Message = "message",
                Columns = new List<BoardColumn>(),
                Rows = new List<BoardRow>()
            };
            var bc2 = new BoardColumn("bc");
            board2.Columns.Add(bc2);
            var issue2 = new Issue
            {
                Key = "123",
                Fields = new Fields
                {
                    Priority = new Priority
                    {
                        Id = "111",
                        Name = "big",
                    },
                    Assignee = new Assignee
                    {
                        DisplayName = "displayName",
                    },
                    Status = new Status
                    {
                        Id = "111",
                        Name = "status",
                    },
                    Description = "description",
                    Summary = "summary",
                }
            };
            bc2.Issues.Add(issue2);

            Assert.True(board1.Equals(board2));
        }
    }
}
