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
using Microsoft.Extensions.Logging;

namespace ESL.CO.React.JiraIntegration
{
    /// <summary>
    /// A class for making requests to Atlassian Jira REST API.
    /// </summary>
    public class JiraClient : IJiraClient
    {

        private readonly ILogger logger;

        public JiraClient(ILogger<JiraClient> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Makes connections to Atlassian Jira.
        /// </summary>
        /// <typeparam name="T">Determines the type of object whose information will be retrieved.</typeparam>
        /// <param name="url">Part of URL required to make different Jira REST API requests.</param>
        /// <param name="id">Board id required for creating board specific connection logs.</param>
        /// <returns>A type specific object corresponding to the JSON response from Jira REST API.</returns>
        public async Task<T> GetBoardDataAsync<T>(string url, int id)
        {
            HttpClient client = new HttpClient();

            #region Credentials
            string credentials = "arumka:Dzukste22";
            #endregion

            //ILogger<JiraClient> logger;
            //logger.LogInformation();

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
        public void SaveToConnectionLog(string url, HttpResponseMessage response, int id)
        {
            var filePath = Path.Combine(@".\data\logs\", id.ToString() + "_jiraConnectionLog.json");
            var logEntry = new JiraConnectionLogEntry(url, response.StatusCode.ToString()); //, (response.ExceptionResponse().Result.Message != null) ? response.ExceptionResponse().Result.Message : "");

            //reads connection log
            var connectionLog = new List<JiraConnectionLogEntry>();
            if (File.Exists(filePath))  
                // PROBLEM IF FILE EXISTS BUT NOT VALID JSON or valid but wrong...
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    connectionLog = JsonConvert.DeserializeObject<List<JiraConnectionLogEntry>>(json);
                }
            }

            //writes appended connection log
            connectionLog.Add(logEntry);
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, connectionLog);
            }

            return;
            // replace list with JiraConnectionLogEntry[] array = new JiraConnectionLogEntry[100];
            // keep track of head when adding
        }

        public void SaveToConnectionLog_UsingLogger(string url, HttpResponseMessage response, int id)
        {

            var logEntry = new JiraConnectionLogEntry(url, response.StatusCode.ToString()); //, (response.ExceptionResponse().Result.Message != null) ? response.ExceptionResponse().Result.Message : "");
            this.logger.LogInformation(url + " " + response.StatusCode.ToString());
        }

        public void SaveToConnectionLog_AsTextFile(string url, HttpResponseMessage response, int id)
        {
            var filePath = Path.Combine(@".\data\logs\", id.ToString() + "_jiraConnectionLog.txt");
            Directory.CreateDirectory(@".\data\logs\");
            using (StreamWriter file = File.AppendText(filePath))
            {
                file.WriteLine(DateTime.Now + "|" + url + "|" + response.StatusCode.ToString());
            }
        }
    }
}
