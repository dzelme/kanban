using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class DbSettings
    {
        public string MongoDbUrl { get; set; }
        public string DatabaseName { get; set; }
        public string StatisticsCollectionName { get; set; }
        public string PresentationsCollectionName { get; set; }
        public string IdCollectionName { get; set; }
        public int NetworkStatisticsEntryCapacity { get; set; }
    }
}
