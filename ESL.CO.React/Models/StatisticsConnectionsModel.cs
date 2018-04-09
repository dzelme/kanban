using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class StatisticsConnectionsModel
    {
        public string Time { get; set; }
        public string Link { get;set; }
        public string ResponseStatus { get; set; }
        public string Exception { get; set; }

        public StatisticsConnectionsModel(string link = "", string responseStatus = "", string exception ="", string time = "")
        {
            string pattern = "dd.MM.yyyy HH:mm:ss";

            Time = (time == "") ? DateTime.Now.ToString(pattern) : DateTime.Parse(time).ToString(pattern);
            Link = link;
            ResponseStatus = responseStatus;
            Exception = exception;
        }
    }
}
