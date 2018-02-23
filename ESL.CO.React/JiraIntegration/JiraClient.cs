using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Encodings;
using System.Text;
using ESL.CO.React.Models;

namespace ESL.CO.React.JiraIntegration
{
    public class JiraClient
    {
        public async Task<T> GetBoardDataAsync<T>(string url)
        {
            HttpClient client = new HttpClient();

              #region Credentials
            string credentials = "adzelme:testTEST0";
            #endregion

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            var response = await client.GetAsync("https://jira.returnonintelligence.com/rest/agile/1.0/" + url);
            if (response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    //return serializer.Deserialize<IssueList>(jsonReader);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }

            return default(T);  //null
            //throw new InvalidOperationException();
        }
    }
}
