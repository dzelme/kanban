using System.Net.Http;
using System.Threading.Tasks;

namespace ESL.CO.React.JiraIntegration
{
    public interface IJiraClient
    {
        Task<T> GetBoardDataAsync<T>(string url, int id = 0);
        void SaveToConnectionLog(string url, HttpResponseMessage response, int id);
    }
}