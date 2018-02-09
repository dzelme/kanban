using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class IssueList
    {
        public string Expand { get; set; }
        public int Total { get; set; }
        public List<Issue> AllIssues { get; set; }
    }
}
