using System.Collections.Generic;
using System.Net.Http;

namespace ESL.CO.React.DbConnection
{
    public interface IDbClient
    {
        int GeneratePresentationId();
        IEnumerable<T> GetList<T>();
        T GetOne<T>(string id);
        void Remove<T>(string id);
        T Save<T>(T entry);
        void Update<T>(string id, T entry);
        void UpdateNetworkStats(string id, string url, HttpResponseMessage response);
    }
}