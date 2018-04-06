using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.Models
{
    public class StatisticsEntry
    {
        [BsonId, BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public int TimesShown { get; set; }
        public DateTime? LastShown { get; set; }
        public Queue<JiraConnectionLogEntry> NetworkStats { get; set; } = new Queue<JiraConnectionLogEntry>();

        public StatisticsEntry() { }

        public StatisticsEntry(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
