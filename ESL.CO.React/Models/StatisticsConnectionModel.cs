using System;

namespace ESL.CO.React.Models
{
    public class StatisticsConnectionModel
    {
        public DateTime Time { get; set; }
        public string Link { get;set; }
        public string ResponseStatus { get; set; }
        public string Exception { get; set; }
    }
}
