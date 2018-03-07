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
        public async Task<T> GetBoardDataAsync<T>(string url, int id = 0)
        {
            HttpClient client = new HttpClient();

            #region Credentials
            string credentials = "adzelme:";
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
                    SaveToConnectionLog(url, response, id);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
            else
            {
                //HttpError error = response.Content.ReadAsStringAsync().Result;
            }

            SaveToConnectionLog(url, response, id);
            return default(T);  //null
            //throw new InvalidOperationException();
        }

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
                    try
                    {
                        connectionLog = JsonConvert.DeserializeObject<List<JiraConnectionLogEntry>>(json);
                    }
                    catch
                    {
                        connectionLog = null;
                    }
                }
            }

            //writes appended connection log
            connectionLog.Add(logEntry);
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, connectionLog);
            }

            // replace list with JiraConnectionLogEntry[] array = new JiraConnectionLogEntry[100];
            // keep track of head when adding
        }
    }


    //public static class HttpResponseMessageExtension
    //{
    //    public static async Task<ExceptionResponse> ExceptionResponse(this HttpResponseMessage httpResponseMessage)
    //    {
    //        string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
    //        ExceptionResponse exceptionResponse = JsonConvert.DeserializeObject<ExceptionResponse>(responseContent);
    //        return exceptionResponse;
    //    }
    //}

    //public class ExceptionResponse
    //{
    //    public string Message { get; set; }
    //    public string ExceptionMessage { get; set; }
    //    public string ExceptionType { get; set; }
    //    public string StackTrace { get; set; }
    //    public ExceptionResponse InnerException { get; set; }
    //}

}
