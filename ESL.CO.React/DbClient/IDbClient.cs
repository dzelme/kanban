using System.Collections.Generic;
using System.Threading.Tasks;
using ESL.CO.React.Models;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        int GeneratePresentationId();
        BoardPresentationDbModel GetAPresentation(string id);
        Task<List<BoardPresentationDbModel>> GetPresentationsListAsync();
        Task<List<StatisticsConnectionsModel>> GetStatisticsConnectionsListAsync(string id);
        Task<List<StatisticsModel>> GetStatisticsListAsync();
        void DeleteAPresentation(string id);
        Task SavePresentationsAsync(BoardPresentation entry);
        Task SaveStatisticsAsync(StatisticsDbModel entry);
    }
}