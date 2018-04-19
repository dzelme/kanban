using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    public interface IJiraClient
    {
        Task<T> GetBoardDataAsync<T>(string url, Credentials credentials, string boardId = "", string presentationId = "");
        Task<IEnumerable<Value>> GetFullBoardList(Credentials credentials = null, string id = null);
    }

}