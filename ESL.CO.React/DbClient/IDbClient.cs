using System.Collections.Generic;
using System.Threading.Tasks;
using ESL.CO.React.Models;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        Task DeletePresentation(string id);
        Task<BoardPresentationDbModel> GetPresentation(string id);
        Task<List<BoardPresentationDbModel>> GetPresentationsListAsync();
        Task<IEnumerable<StatisticsBoardModel>> GetStatisticsBoardListAsync();
        Task<List<StatisticsConnectionModel>> GetStatisticsConnectionsListAsync(string id);
        Task<IEnumerable<StatisticsPresentationModel>> GetStatisticsPresentationListAsync();
        Task SavePresentationsAsync(BoardPresentation entry);
        Task SaveStatisticsAsync(StatisticsDbModel entry);
    }
}