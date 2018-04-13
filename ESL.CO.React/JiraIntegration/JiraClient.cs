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

        /// <summary>
        /// Obtains data from Atlassian Jira.
        /// </summary>
        /// <typeparam name="T">Determines the type of object whose information will be retrieved.</typeparam>
        /// <param name="url">Request specific part of the URL required to make different Jira REST API requests.</param>
        /// <param name="credentials">Jira user login credentials for making requests.</param>
        /// <param name="boardId">Board id required for saving board specific statistics.</param>
        /// <param name="presentationId">Presentation id required for saving presentation specific statistics.</param>
        /// <returns>A type specific object corresponding to the JSON response from Jira REST API.</returns>
        public async Task<T> GetBoardDataAsync<T>(string url, Credentials credentials, string boardId = "", string presentationId = "")
        {
            var baseUri = new Uri("https://jira.returnonintelligence.com/rest/");
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUri, url));
            var credentialsString = credentials.Username + ":" + credentials.Password;
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(credentialsString)));
            
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    if (!string.IsNullOrEmpty(boardId))  // not to store data about general Jira requests (not specific to a particular board)
                    {
                        dbClient.SaveStatisticsAsync(
                            new StatisticsDbModel("connection", presentationId, boardId, url, response.StatusCode.ToString(), response.ReasonPhrase));
                    }
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
            else
            {
                //HttpError error = response.Content.ReadAsStringAsync().Result;
            }

            if (!string.IsNullOrEmpty(boardId))  // not to store data about general Jira requests (not specific to a particular board)
            {
                dbClient.SaveStatisticsAsync(
                    new StatisticsDbModel("connection", presentationId, boardId, url, response.StatusCode.ToString(), response.ReasonPhrase));
            }

            return default(T);  //null
        }

        /// <summary>
        /// Obtains a list of boards available to a particular user.
        /// </summary>
        /// <param name="credentials">Jira user login credentials for making requests.</param>
        /// <returns>A full list of boards (objects containing board data) that are available to the user whose credentials were passed as a parameter.</returns>
        public async Task<IEnumerable<Value>> GetFullBoardList(Credentials credentials = null, string id = null)
        {
            if (id != null) { credentials = (await dbClient.GetPresentation(id))?.Credentials; }

            var boardList = await GetBoardDataAsync<BoardList>("agile/1.0/board/", credentials);
            if (boardList == null) { return new List<Value>(); }

            FullBoardList fullBoardList = new FullBoardList
            {
                Values = new List<Value>()
            };
            fullBoardList.Values.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString(), credentials);
                if (boardList == null) { return fullBoardList.Values; }
                fullBoardList.Values.AddRange(boardList.Values);
            }

            return fullBoardList.Values;
        }
    }
}
