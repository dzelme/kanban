using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/issue
    public class Issue
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Assignee { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }

        public Issue(string i, string k, string stat, string p, string a, string sum, string l)
        {
            Id = i;
            Key = k;
            Status = stat;
            Priority = p;
            Assignee = a;
            Summary = sum;
            Link = l;
        }
    }
}
