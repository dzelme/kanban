using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Fields
    {
        public Priority Priority { get; set; }
        public Assignee Assignee { get; set; }
        public Status Status { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }


        public Fields()
        {
            Priority = new Priority();
            Assignee = new Assignee();
            Status = new Status();
            Description = string.Empty;
            Summary = string.Empty;
        }
    }
}
