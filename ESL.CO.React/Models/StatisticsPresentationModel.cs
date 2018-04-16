using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ESL.CO.React.Models
{
    public class StatisticsPresentationModel
    {
        public string PresentationId { get; set; }
        public string Title { get; set; }
        public FullBoardList Boards { get; set; }
        public int TimesShown { get; set; }
        public DateTime LastShown { get; set; }

        public StatisticsPresentationModel()
        {
            PresentationId = "";
            Title = "";
            Boards = new FullBoardList { Values = new List<Value>() };
            TimesShown = 0;
            LastShown = DateTime.Now;
        }
    }
}
