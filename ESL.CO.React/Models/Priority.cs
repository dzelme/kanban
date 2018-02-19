using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Priority
    {
        //public string self { get; set; }
        //public string iconUrl { get; set; }
        public string Name { get; set; }  //e.g., critical
        public string Id { get; set; }

        public Priority()
        {
            Name = string.Empty;
            Id = string.Empty;
        }
    }
}
