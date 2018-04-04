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
        [BsonId]
        public string Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("TimesShown")]
        public int TimesShown { get; set; }
        [BsonElement("LastShown")]
        public DateTime? LastShown { get; set; }
        [BsonElement("NetworkStats")]
        public Queue<JiraConnectionLogEntry> NetworkStats { get; set; }

        public StatisticsEntry(string id = "0", string name = "")  //IOptions<DbSettings> dbSettings, 
        {
            //this.dbSettings = dbSettings;
            Id = id;
            Name = name;
            TimesShown = 0;
            LastShown = null;
            NetworkStats = new Queue<JiraConnectionLogEntry>();  // (dbSettings.Value.NetworkStatisticsEntryCapacity);
        }
    }
}
