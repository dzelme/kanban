using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;


namespace ESL.CO.React.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IJiraClient jiraClient;
        private readonly IAppSettings appSettings;
        private readonly IMemoryCache cache;

        public SampleDataController(IMemoryCache cache, IJiraClient jiraClient, IAppSettings appSettings)
        {
            this.jiraClient = jiraClient;
            this.appSettings = appSettings;
            this.cache = cache;
        }

        //Obtain board list from Jira, passes to React
        [HttpGet("[action]")]
        public async Task<IEnumerable<Models.Value>> BoardList()
        {
            var boardList = await jiraClient.GetBoardDataAsync<BoardList>("board/");
            if (boardList == null)
            {
                return this.appSettings.GetSavedAppSettings()?.AllValues;
            }  //

            FullBoardList boardSettings = new FullBoardList();
            boardSettings.AllValues.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await jiraClient.GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString());
                //if (boardList == null) { return null; }  //
                if (boardList == null)
                {
                    boardSettings = AppSettings.MergeSettings(appSettings.GetSavedAppSettings(), boardSettings);
                    appSettings.SaveAppSettings(boardSettings);
                    return boardSettings.AllValues;
                }
                boardSettings.AllValues.AddRange(boardList.Values);
            }

            boardSettings = AppSettings.MergeSettings(appSettings.GetSavedAppSettings(), boardSettings);
            appSettings.SaveAppSettings(boardSettings);
            return boardSettings.AllValues;
        }

        //obtain a full kanban board
        [HttpGet("[action]")]
        public async Task<Models.Board> BoardData(int id)
        {
            var creator = new BoardCreator();
            var b = creator.CreateBoardModel(id, cache);
            Board board = null;
            try
            {
                board = await b;
                if (NeedsRedraw(board))
                {
                    board.HasChanged = true;
                    this.cache.Set<Board>(id, board);
                    return board;
                }
                else return board;
            }
            catch
            {
                //
                Console.Write("no internet");
            }
            return board;
        }

        public bool NeedsRedraw(Board board)
        {
            if (!this.cache.TryGetValue(board.Id, out Board cachedBoard)) { return true; }
            if (board.Equals(cachedBoard)) { return false; }
            else return true;
        }

        /// <summary>
        /// obtain a full kanban board
        /// </summary>
        /// <param name="id"></param>
        [HttpPost("[action]")]
        public void IncrementTimesShown([FromBody]int id)
        {
            var boardList = new FullBoardList();
            boardList = appSettings.GetSavedAppSettings();


            //updates the board's statistics
            foreach (var value in boardList.AllValues)
            {
                if (value.Id == id)
                {
                    checked { value.TimesShown++; } //checked...
                    value.LastShown = DateTime.Now;
                    appSettings.SaveAppSettings(boardList);
                    return;
                }
            }
            return;
        }

        /// <summary>
        /// return statistics file entries
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public List<JiraConnectionLogEntry> NetworkStatistics(int id)
        {
            var filePath = Path.Combine(@".\data\logs\", id.ToString() + "_jiraConnectionLog.json");
            var connectionLog = new List<JiraConnectionLogEntry>();
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    connectionLog = JsonConvert.DeserializeObject<List<JiraConnectionLogEntry>>(json);
                }
            }
            return connectionLog;
        }
    }
}
