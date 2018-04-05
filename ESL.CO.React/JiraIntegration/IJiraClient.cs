using System.Net.Http;
using System.Threading.Tasks;

namespace ESL.CO.React.JiraIntegration
{
    public interface IJiraClient
    {
        Task<T> GetBoardDataAsync<T>(string url, string credentials, int id = 0);
        void SaveToConnectionLog_AsTextFile(string url, HttpResponseMessage response, int id);
        void TruncateLogFile(string filePath);
    }

}