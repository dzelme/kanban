using System.Collections.Generic;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using MongoDB.Bson;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        int GeneratePresentationId();
        Task DeletePresentation(string id);
        Task<BoardPresentationDbModel> GetPresentation(string id);
        Task<List<BoardPresentationDbModel>> GetPresentationsListAsync();
        Task<List<StatisticsConnectionsModel>> GetStatisticsConnectionsListAsync(string id);
        Task<IEnumerable<StatisticsModel>> GetStatisticsListAsync();
        Task SavePresentationsAsync(BoardPresentation entry);
        Task SaveStatisticsAsync(StatisticsDbModel entry);
    }
}