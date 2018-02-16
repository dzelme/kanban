using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class FullIssueList //: IssueList
    {
        public List<Issue> AllIssues { get; set; }

        public FullIssueList()
        {
            AllIssues = new List<Issue>();
        }
    }
}
