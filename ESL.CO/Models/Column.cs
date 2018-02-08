using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Column
    {
        public string Title { get; set; }
        public string ParentBoardId { get; set; }
        public List<Issue> ColumnIssues { get; set; }
        public int IssueCount { get; set; }
    }
}
