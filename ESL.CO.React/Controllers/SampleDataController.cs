using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Caching.Memory;


namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A data controller.
    /// </summary>
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IJiraClient jiraClient;
        private readonly IAppSettings appSettings;
        private readonly IMemoryCache cache;
        private readonly IBoardCreator boardCreator;

        public SampleDataController(IMemoryCache cache, IJiraClient jiraClient, IAppSettings appSettings, IBoardCreator boardCreator)
        {
            this.jiraClient = jiraClient;
            this.appSettings = appSettings;
            this.cache = cache;
            this.boardCreator = boardCreator;
        }

        /// <summary>
        /// Obtains the full currently available board list from Jira REST API.
        /// </summary>
        /// <returns>A task of obtaining the list of all currently available Kanban boards.</returns>
        [HttpGet("[action]")]
        public async Task<IEnumerable<Value>> BoardList()
        {
            var boardList = await jiraClient.GetBoardDataAsync<BoardList>("board/");
            if (boardList == null)
            {
                return this.appSettings.GetSavedAppSettings()?.Values;
            }  //

            FullBoardList fullBoardList = new FullBoardList();
            fullBoardList.Values.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await jiraClient.GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString());
                //if (boardList == null) { return null; }  //
                if (boardList == null)
                {
                    fullBoardList = AppSettings.MergeSettings(appSettings.GetSavedAppSettings(), fullBoardList);
                    appSettings.SaveAppSettings(fullBoardList);
                    return fullBoardList.Values;
                }
                fullBoardList.Values.AddRange(boardList.Values);
            }

            fullBoardList = AppSettings.MergeSettings(appSettings.GetSavedAppSettings(), fullBoardList);
            appSettings.SaveAppSettings(fullBoardList);
            return fullBoardList.Values;
        }

        /// <summary>
        /// Gets board data, checks if the data has changed (compared to cached version), saves to cache if it has.
        /// </summary>
        /// <param name="id">Id of the board whose data will be returned</param>
        /// <returns>Board information.</returns>
        [HttpGet("[action]")]
        public async Task<Board> BoardData(int id)
        {
            
            var b = boardCreator.CreateBoardModel(id, cache);
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

        /// <summary>
        /// Checks if board has to be redrawn.
        /// </summary>
        /// <param name="board">Board data to be compared with the cached version.</param>
        /// <returns>True or false.</returns>
        public bool NeedsRedraw(Board board)
        {
            if (!this.cache.TryGetValue(board.Id, out Board cachedBoard)) { return true; }
            if (board.Equals(cachedBoard)) { return false; }
            else return true;
        }

        /// <summary>
        /// Increases the statistics counter each time the board is shown.
        /// </summary>
        /// <param name="id">Id of the board whose counter will be increased.</param>
        [HttpPost("[action]")]
        public void IncrementTimesShown([FromBody]int id)
        {
            var boardList = new FullBoardList();
            boardList = appSettings.GetSavedAppSettings();

            //updates the board's statistics
            foreach (var value in boardList.Values)
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
        /// Gets connection log entries.
        /// </summary>
        /// <param name="id">Id of the board whose connection log will be returned.</param>
        /// <returns>List of connection log entries.</returns>
        //[HttpGet("[action]")]
        //public List<JiraConnectionLogEntry> NetworkStatistics(int id)
        //{
        //    var filePath = Path.Combine(@".\data\logs\", id.ToString() + "_jiraConnectionLog.json");
        //    var connectionLog = new List<JiraConnectionLogEntry>();
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        using (StreamReader r = new StreamReader(filePath))
        //        {
        //            string json = r.ReadToEnd();
        //            connectionLog = JsonConvert.DeserializeObject<List<JiraConnectionLogEntry>>(json);
        //        }
        //    }
        //    return connectionLog;
        //}

        [HttpGet("[action]")]
        public List<JiraConnectionLogEntry> NetworkStatistics(int id)
        {
            var filePath = Path.Combine(@".\data\logs\", id.ToString() + "_jiraConnectionLog.txt");
            var connectionLog = new List<JiraConnectionLogEntry>();
            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    var logEntry = r.ReadLine();
                    while (logEntry != null)
                    {
                        string[] logEntryField = logEntry.Split('|');
                        connectionLog.Add(new JiraConnectionLogEntry(
                            logEntryField[1],
                            logEntryField[2],
                            (logEntryField.Length > 3) ? logEntryField[3] : "",
                            logEntryField[0]
                        ));
                        logEntry = r.ReadLine();
                    }
                }
            }
            return connectionLog;
        }
    }
}
