using System;

namespace ESL.CO.React.Models
{
    public class StatisticsModel
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public int TimesShown { get; set; }  // groupby boardid - count
        public string LastShown { get; set; }  // group by boardid - last
    }
}