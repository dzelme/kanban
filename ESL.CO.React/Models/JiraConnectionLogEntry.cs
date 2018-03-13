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
            this.Time = (time == "") ? DateTime.Now.ToString() : time;
            this.Link = link;
            this.ResponseStatus = responseStatus;
            this.Exception = exception;

            //required because javascript date.parse only understands months first (mm/dd/yyyy) format
            string[] timeParts = Time.Split('.');  //13.03.2018 12:04:19
            Time = timeParts[1] + "." + timeParts[0] + "." + timeParts[2];  //03.13.2018 12:04:19
        }
    }
}
