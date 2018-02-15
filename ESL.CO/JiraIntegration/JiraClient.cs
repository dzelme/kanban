﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Encodings;
using System.Text;
using ESL.CO.Models;

namespace ESL.CO.JiraIntegration
{
    public class JiraClient
    {

        HttpClient client = new HttpClient();

        #region Credentials
        string credentials = "adzelme:testTEST0";
        #endregion

        string JiraLink = "https://jira.returnonintelligence.com/rest/agile/1.0/";

        public async Task<IssueList> GetIssueListAsync(string url)
        {

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            var response = await client.GetAsync(JiraLink + url);
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

        public async Task<BoardList> GetBoardListAsync(string url)
        {

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            var response = await client.GetAsync(JiraLink + url);
            if(response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<BoardList>(jsonReader);
                }
            }

            throw new InvalidOperationException();
        }

        public async Task<BoardConfig> GetBoardConfigAsync(string url)
        {
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            var response = await client.GetAsync(JiraLink + url);
            if (response.IsSuccessStatusCode)
            {
                var serializer = new JsonSerializer();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<BoardConfig>(jsonReader);
                }
            }

            throw new InvalidOperationException();
        }
    }
}
