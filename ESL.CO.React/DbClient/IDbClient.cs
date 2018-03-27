using System.Collections.Generic;
using System.Net.Http;
using ESL.CO.React.Models;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        StatisticsEntry GetStatisticsEntry(string id);
        IEnumerable<StatisticsEntry> GetStatisticsList();
        void RemoveStatisticsEntry(string id);
        StatisticsEntry SaveStatisticsEntry(StatisticsEntry entry);
        void UpdateNetworkStats(string id, string url, HttpResponseMessage response);
        void UpdateStatisticsEntry(StatisticsEntry entry);
    }
}