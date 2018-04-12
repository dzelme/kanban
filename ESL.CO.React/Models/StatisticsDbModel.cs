using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.Models
{
    public class StatisticsDbModel : StatisticsConnectionModel
    {
        [BsonId, BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }  // database entry primary key
        public string ItemId { get; set; }  // board or presentation id
        public string Type { get; set; }

        public StatisticsDbModel() { }

        public StatisticsDbModel(string id, string type, string link = "", string responseStatus = "", string exception = "", string time = "")
        {
            string pattern = "dd.MM.yyyy HH:mm:ss";

            ItemId = id;
            Type = type;
            Time = (time == "") ? DateTime.Now.ToString(pattern) : DateTime.Parse(time).ToString(pattern);
            Link = link;
            ResponseStatus = responseStatus;
            Exception = exception;
        }
    }
}
