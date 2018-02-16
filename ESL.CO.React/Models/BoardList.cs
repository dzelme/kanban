using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board
    public class BoardList
    {
        public int MaxResults { get; set; }
        public int StartAt { get; set; }
        public bool IsLast { get; set; }
        public List<Value> Values { get; set; }
    }

}