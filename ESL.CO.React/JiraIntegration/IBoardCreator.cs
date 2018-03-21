using System.Threading.Tasks;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ESL.CO.React.JiraIntegration
{
    public interface IBoardCreator
    {
        Task<Board> CreateBoardModel(int id, string credentials, IMemoryCache cache);
    }
}