using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Helpers
{
    public class Issue
    {
        private string id;
        private string status;
        private string priority;
        private string assignee;
        private string summary;
        private string link;

        public Issue(string i, string stat, string p, string a, string sum, string l)
        {
            Id = i;
            Status = stat;
            Priority = p;
            Assignee = a;
            Summary = sum;
            Link = l;
        }

        public string Id { get => id; set => id = value; }
        public string Status { get => status; set => status = value; }
        public string Priority { get => priority; set => priority = value; }
        public string Assignee { get => assignee; set => assignee = value; }
        public string Summary { get => summary; set => summary = value; }
        public string Link { get => link; set => link = value; }
    }
}
