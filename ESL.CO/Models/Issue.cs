using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Issue
    {
        public string Id { get; set; }
        public string Key { get; set; }  //used to generate link to the particular issue
        public Fields Fields { get; set; }
    }

}
