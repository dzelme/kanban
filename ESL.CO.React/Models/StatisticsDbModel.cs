using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.Models
{
    public class StatisticsDbModel
    {
        [BsonId, BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string BoardId { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public string Link { get; set; }
        public string ResponseStatus { get; set; }
        public string Exception { get; set; }

        public StatisticsDbModel() { }

        public StatisticsDbModel(string id, string link = "", string responseStatus = "", string exception = "", string time = "")
        {
            string pattern = "dd.MM.yyyy HH:mm:ss";

            BoardId = id;
            Type = "p";
            Time = (time == "") ? DateTime.Now.ToString(pattern) : DateTime.Parse(time).ToString(pattern);
            Link = link;
            ResponseStatus = responseStatus;
            Exception = exception;
        }
    }
}
