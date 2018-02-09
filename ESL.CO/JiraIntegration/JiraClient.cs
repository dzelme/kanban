using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Encodings;
using System.Text;

namespace ESL.CO.JiraIntegration
{
    public class JiraClient
    {
        public async Task<IssueList> GetIssuesAsync()
        {
            HttpClient client = new HttpClient();

            #region Credentials
            string credentials = "adzelme:testTEST0";
            #endregion

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            var response = await client.GetAsync("https://jira.returnonintelligence.com/rest/agile/1.0/board/961/issue");
            if(response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<IssueList>(jsonReader);
                }
            }

            throw new InvalidOperationException();
        }
    }

    public class IssueList
    {
        public string Expand { get; set; }
        public Issue[] Issues { get; set; }
        public int MaxResults { get; set; }
        public int StartAt { get; set; }
        public int Total { get; set; }
    }

    public class Issue
    {
        public string Id { get; set; }
        public string Key { get; set; }
    }
}
