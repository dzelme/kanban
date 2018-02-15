using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Board
    {
        public int Id { get; set; }
        public List<BoardColumn> Columns { get; set; }
        public List<BoardRow> Rows { get; set; }  //

        public Board(int id)
        {
            Id = id;
            Columns = new List<BoardColumn>();
            Rows = new List<BoardRow>();
        }
    }

    public class BoardColumn
    {
        public string Name { get; set; }
        public List<Issue> Issues { get; set; }

        public BoardColumn(string name)
        {
            Name = name;
            Issues = new List<Issue>();
        }
    }

    public class BoardRow  //
    {
        public List<Issue> IssueRow { get; set; }

        public BoardRow()
        {
            IssueRow = new List<Issue>();
        }
    }
}
