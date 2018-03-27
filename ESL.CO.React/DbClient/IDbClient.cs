using System.Collections.Generic;
using ESL.CO.React.Models;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        StatisticsEntry GetStatisticsEntry(string id);
        IEnumerable<StatisticsEntry> GetStatisticsList();
        void RemoveStatisticsEntry(string id);
        StatisticsEntry SaveStatisticsEntry(StatisticsEntry entry);
        void UpdateStatisticsEntry(string id, StatisticsEntry entry);
    }
}