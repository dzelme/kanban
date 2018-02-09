using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Issue
    {
        public string ID { get; set; }
        public string Key { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Assignee { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
    }
}
