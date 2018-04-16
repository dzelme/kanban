using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ESL.CO.React.Models
{
    public class StatisticsDbModel
    {
        [BsonId, BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }  // database entry primary key
        public string PresentationId { get; set; }
        public string BoardId { get; set; }
        public string Type { get; set; }
        [BsonElement("Time")]
        public DateTime Time { get; set; }
        public string Link { get; set; }
        public string ResponseStatus { get; set; }
        public string Exception { get; set; }

        public StatisticsDbModel() { }

        public StatisticsDbModel(string type, string presentationId, string boardId = "", string link = "", string responseStatus = "", string exception = "", string time = "")
        {
            PresentationId = presentationId;
            BoardId = boardId;
            Type = type;
            Time = (time == "") ? DateTime.Now : DateTime.Parse(time);
            Link = link;
            ResponseStatus = responseStatus;
            Exception = exception;
        }
    }
}
