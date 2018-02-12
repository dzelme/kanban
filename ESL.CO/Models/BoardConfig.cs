using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;

namespace ESL.CO.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/configuration

    public class ColumnStatus
    {
        public string Id { get; set; }
        //public string self { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public List<ColumnStatus> Statuses { get; set; }  //which issues shown in this column
        //public int? Max { get; set; }  //max number of issues in one column
    }

    public class ColumnConfig
    {
        public List<Column> Columns { get; set; }
        //public string constraintType { get; set; }
    }

    public class BoardConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        //public string self { get; set; }
        //public Filter filter { get; set; }
        //public SubQuery subQuery { get; set; }
        public ColumnConfig ColumnConfig { get; set; }
        //public Ranking ranking { get; set; }
    }


    public class Board
    {
        public int Id { get; set; }
        public List<Col> Columns { get; set; }

        public Board(int id)
        {
            Id = id;
            Columns = new List<Col>();
            //Columns.AddRange(ColumnConfig.Columns)
        }
    }

    public class Col
    {
        public string Name { get; set; }
        public List<Issue> Issues { get; set; }

        public Col(string name)
        {
            Name = name;
            Issues = new List<Issue>();
        }
    }

}
