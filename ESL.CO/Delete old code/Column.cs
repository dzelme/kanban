using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    //obtained from each https://jira.returnonintelligence.com/rest/agile/1.0/board/963/configuration
    public class Column2
    {
        public string Name { get; set; }
        public string[] Statuses { get; set; }  //possible list //ids that go into column
        //public int Max { get; set; }
        //public IssueList ColumnIssues { get; set; }

        public List<Issue> Issues { get; set; }

        
        //not needed with httpClient?
        public Column2(string name)
        {
            this.Name = name;
            Issues = new List<Issue>();
        }
        
    }
}
