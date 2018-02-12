using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Status
    {
        public string Self { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public IssueStatusCategory StatusCategory { get; set; }
    }
}
