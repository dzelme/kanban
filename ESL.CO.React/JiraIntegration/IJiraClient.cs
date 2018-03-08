using System.Net.Http;
using System.Threading.Tasks;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// An interface for JiraClient class???
    /// </summary>
    public interface IJiraClient
    {
        Task<T> GetBoardDataAsync<T>(string url, int id = 0);
        void SaveToConnectionLog(string url, HttpResponseMessage response, int id);
    }
}