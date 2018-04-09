using System.Collections.Generic;
using System.Threading.Tasks;
using ESL.CO.React.Models;
using MongoDB.Bson;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        void DeleteAPresentation(string id);
        int GeneratePresentationId();
        BoardPresentationDbModel GetAPresentation(string id);
        Task<List<BoardPresentationDbModel>> GetPresentationsListAsync();
        Task<List<StatisticsConnectionsModel>> GetStatisticsConnectionsListAsync(string id);
        Task<List<StatisticsModel>> GetStatisticsListAsync();
        Task SavePresentationsAsync(BoardPresentation entry);
        Task SaveStatisticsAsync(StatisticsDbModel entry);
    }
}