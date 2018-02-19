using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Issue
    {
        //public string expand { get; set; }
        //public string id { get; set; }
        //public string self { get; set; }
        public string Key { get; set; }  //used to generate link to the particular issue
        public Fields Fields { get; set; }

        public Issue()
        {
            Key = string.Empty;
            Fields = new Fields();
        }
    }

}
