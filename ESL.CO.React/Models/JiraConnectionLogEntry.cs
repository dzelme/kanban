using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class JiraConnectionLogEntry
    {
        public DateTime Time { get; set; }
        public string Link { get;set; }
        public string ResponseStatus { get; set; }
        public string Exception { get; set; }

        public JiraConnectionLogEntry(string link = "", string responseStatus = "", string exception ="")
        {
            Time = DateTime.Now;
            this.Link = link;
            this.ResponseStatus = responseStatus;
            this.Exception = exception;
        }
    }
}
