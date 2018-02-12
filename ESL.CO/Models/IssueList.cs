using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/issue

    public class IssueList
    {
        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }
        public List<Issue> Issues { get; set; }
    }

    public class FullIssueList //: IssueList
    {
        public List<Issue> AllIssues { get; set; }

        public FullIssueList()
        {
            AllIssues = new List<Issue>();
        }
    }
}



