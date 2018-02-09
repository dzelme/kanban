using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Column
    {
        public string Name { get; set; }
        public int Max { get; set; }
        public IssueList ColumnIssues { get; set; }
    }
}
