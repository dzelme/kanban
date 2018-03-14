using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class JiraConnectionLogEntry
    {
        public string Time { get; set; }
        public string Link { get;set; }
        public string ResponseStatus { get; set; }
        public string Exception { get; set; }

        public JiraConnectionLogEntry(string link = "", string responseStatus = "", string exception ="", string time = "")
        {
            //required because javascript date.parse only understands months first (mm.dd.yyyy) format;
            string pattern = "MM.dd.yyyy HH:mm:ss";

            this.Time = (time == "") ? DateTime.Now.ToString(pattern) : DateTime.Parse(time).ToString(pattern);
            this.Link = link;
            this.ResponseStatus = responseStatus;
            this.Exception = exception;
        }
    }
}
