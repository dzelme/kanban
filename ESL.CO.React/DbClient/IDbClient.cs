using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ESL.CO.React.Models;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        int GeneratePresentationId();
        T GetOne<T>(string id);
        Task<List<BoardPresentationDbModel>> GetPresentationsListAsync();
        Task<List<JiraConnectionLogEntry>> GetStatisticsConnectionsListAsync(string id);
        Task<List<StatisticsModel>> GetStatisticsListAsync();
        void Remove<T>(string id);
        Task SavePresentationsAsync(BoardPresentation entry);
        Task SaveStatisticsAsync(Statistics entry);
        void UpdateNetworkStats(string id, string url, HttpResponseMessage response);
    }
}