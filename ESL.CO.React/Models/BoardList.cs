using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board

    public class Value
    {
        public int Id { get; set; }
        //public string Self { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class BoardList
    {
        public int MaxResults { get; set; }
        public int StartAt { get; set; }
        public bool IsLast { get; set; }
        public List<Value> Values { get; set; }
    }

    public class FullBoardList //: BoardList
    {
        public List<Value> AllValues { get; set; }

        public FullBoardList()
        {
            AllValues = new List<Value>();
        }
    }
}