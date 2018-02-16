using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Assignee
    {
        public string Name { get;set;}
        public string Key { get; set; }
        public string Self { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }

        public Assignee()
        {
            DisplayName = string.Empty;
        }
    }
}
