using System.Collections.Generic;
using System.Threading.Tasks;
using ESL.CO.React.Models;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        Task DeletePresentation(string id);
        Task<BoardPresentationDbModel> GetPresentation(string id);
        Task<IEnumerable<BoardPresentationDbModel>> GetPresentationsListAsync();
        Task<IEnumerable<StatisticsBoardModel>> GetStatisticsBoardListAsync(string presentationId);
        Task<List<StatisticsConnectionModel>> GetStatisticsConnectionsListAsync(string presentationId, string boardId);
        Task<IEnumerable<StatisticsPresentationModel>> GetStatisticsPresentationListAsync();
        Task<Task> SavePresentationsAsync(BoardPresentation entry);
        Task SaveStatisticsAsync(StatisticsDbModel entry);
    }
}