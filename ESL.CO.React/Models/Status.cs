using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Status
    {
        //public string self { get; set; }
        //public string description { get; set; }
        //public string iconUrl { get; set; }
        public string Id { get; set; }  //which column belongs to
        public string Name { get; set; }  //e.g., to do, backlog
      
        //public StatusCategory statusCategory { get; set; }

        public Status()
        {
            Name = string.Empty;
            Id = string.Empty;
        }
    }
}
