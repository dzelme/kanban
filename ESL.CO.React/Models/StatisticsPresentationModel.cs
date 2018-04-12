using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ESL.CO.React.Models
{
    public class StatisticsPresentationModel
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public FullBoardList Boards { get; set; }
        public int TimesShown { get; set; }
        public string LastShown { get; set; }

        public StatisticsPresentationModel()
        {
            ItemId = "";
            Title = "";
            Boards = new FullBoardList { Values = new List<Value>() };
            TimesShown = 0;
            LastShown = "";
        }
    }
}
