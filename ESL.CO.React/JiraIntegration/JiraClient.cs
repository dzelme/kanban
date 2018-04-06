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
        public async Task<T> GetBoardDataAsync<T>(string url, string credentials, string id = "")
        {
            var baseUri = new Uri("https://jira.returnonintelligence.com/rest/");
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
                    if (id != "")
                    {
                        dbClient.SaveStatisticsAsync(
                            new Statistics(id, url, response.StatusCode.ToString(), response.ReasonPhrase));
                    }
                    //dbClient.UpdateNetworkStats(id.ToString(), url, response);
                    //SaveToConnectionLog_AsTextFile(url, response, id);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
            else
            {
                //HttpError error = response.Content.ReadAsStringAsync().Result;
            }

            if (id != "")
            {
                dbClient.SaveStatisticsAsync(
                    new Statistics(id, url, response.StatusCode.ToString(), response.ReasonPhrase));
            }
            //dbClient.UpdateNetworkStats(id.ToString(), url, response);
            //SaveToConnectionLog_AsTextFile(url, response, id);
            return default(T);  //null
            //throw new InvalidOperationException();
        }

        public async Task<IEnumerable<Value>> GetFullBoardList(Credentials credentials)
        {
            var credentialsString = credentials.Username + ":" + credentials.Password;

            var boardList = await GetBoardDataAsync<BoardList>("agile/1.0/board/", credentialsString);
            //if (boardList == null)
            //{
            //    return dbClient.GetOne<UserSettingsDbEntry>(credentials.Username)?.BoardSettingsList?.Values;
            //}  //

            FullBoardList fullBoardList = new FullBoardList
            {
                Values = new List<Value>()
            };
            fullBoardList.Values.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString(), credentialsString);
                //if (boardList == null)
                //{
                //    // settings not stored anymore..
                //    fullBoardList = AppSettings.MergeSettings(dbClient.GetOne<UserSettingsDbEntry>(credentials.Username)?.BoardSettingsList, fullBoardList, userSettings);
                //    return fullBoardList.Values;
                //    //dbClient.Update(credentials.Username, new UserSettingsDbEntry
                //    //{
                //    //    Id = credentials.Username,
                //    //    BoardSettingsList = fullBoardList
                //    //});
                //    //return fullBoardList.Values;
                //}
                fullBoardList.Values.AddRange(boardList.Values);
            }

            //fullBoardList = AppSettings.MergeSettings(dbClient.GetOne<UserSettingsDbEntry>(credentials.Username)?.BoardSettingsList, fullBoardList, userSettings);
            //dbClient.Update(credentials.Username, new UserSettingsDbEntry
            //{
            //    Id = credentials.Username,
            //    BoardSettingsList = fullBoardList
            //});
            return fullBoardList.Values;
        }
    }
}
