using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using ESL.CO.React.Models;
using ESL.CO.React.DbConnection;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for making requests to Atlassian Jira REST API.
    /// </summary>
    public class JiraClient : IJiraClient
    {
        private HttpClient client = new HttpClient();
        private readonly IOptions<Paths> paths;
        private readonly IDbClient dbClient;

        public JiraClient(IOptions<Paths> paths, IDbClient dbClient)
        {
            this.paths = paths;
            this.dbClient = dbClient;
        }
          /*  client.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(auth_info)));*/
        
            /// <summary>
            /// Makes connections to Atlassian Jira.
            /// </summary>
            /// <typeparam name="T">Determines the type of object whose information will be retrieved.</typeparam>
            /// <param name="url">Part of URL required to make different Jira REST API requests.</param>
            /// <param name="id">Board id required for creating board specific connection logs.</param>
            /// <returns>A type specific object corresponding to the JSON response from Jira REST API.</returns>
        public async Task<T> GetBoardDataAsync<T>(string url, string credentials, int id)
        {
            var baseUri = new Uri("https://jira.returnonintelligence.com/rest/agile/1.0/");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, url));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));
            
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    dbClient.UpdateNetworkStats(id.ToString(), url, response);

                    //SaveToConnectionLog_AsTextFile(url, response, id);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
            else
            {
                //HttpError error = response.Content.ReadAsStringAsync().Result;
            }

            dbClient.UpdateNetworkStats(id.ToString(), url, response);
            //SaveToConnectionLog_AsTextFile(url, response, id);
            return default(T);  //null
            //throw new InvalidOperationException();
        }

        public async Task<T> GetColorDataAsync<T>(string url, string credentials, int id)
        {
            var baseUri = new Uri("https://jira.returnonintelligence.com/rest/greenhopper/1.0/cardcolors/");

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, url));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    SaveToConnectionLog_AsTextFile(url, response, id);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
            else
            {
                //HttpError error = response.Content.ReadAsStringAsync().Result;
            }

            SaveToConnectionLog_AsTextFile(url, response, id);
            return default(T);  //null
            //throw new InvalidOperationException();
        }


        /// <summary>
        /// Adds an entry to the appropriate connection log file.
        /// </summary>
        /// <param name="url">Jira REST API request URL to be logged.</param>
        /// <param name="response">Request's HTTP response message.</param>
        /// <param name="id">Board id used for saving to the appropriate log file.</param>
        public void SaveToConnectionLog_AsTextFile(string url, HttpResponseMessage response, int id)
        {
            Directory.CreateDirectory(paths.Value.LogDirectoryPath);
            var filePath = Path.Combine(paths.Value.LogDirectoryPath, id.ToString() + "_jiraConnectionLog.txt");
            using (StreamWriter file = File.AppendText(filePath))
            {
                file.WriteLine(DateTime.Now + "|" + url + "|" + response.StatusCode.ToString());
            }
            TruncateLogFile(filePath);
        }

        /// <summary>
        /// Truncates the appropriate connection log file.
        /// </summary>
        /// <param name="filePath">Path on the the server to the appropriate log file to be truncated.</param>
        public void TruncateLogFile(string filePath)
        {
            int maxLogLines = 100;  // should be outside...
            var queue = new Queue<string>(maxLogLines);
            
            using (StreamReader r = new StreamReader(filePath))
            {
                while(!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    if (queue.Count() >= maxLogLines) queue.Dequeue();
                    queue.Enqueue(line);
                } 
            }

            if (queue.Count() >= maxLogLines)
            {
                using (StreamWriter file = new StreamWriter(filePath, false))
                {
                    for (int i = 0; i < maxLogLines; i++)
                    {
                        file.WriteLine(queue.Dequeue().ToString());
                    }
                }
            }

            return;
        }
    }
}
