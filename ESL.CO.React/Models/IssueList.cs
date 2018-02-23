using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/issue

    public class IssueList
    {
        //public string expand { get; set; }
        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }
        public List<Issue> Issues { get; set; }

        public IssueList()
        {
            StartAt = 0;
            MaxResults = 50;
            Total = 0;
            Issues = new List<Issue>();
        }
    }

}



