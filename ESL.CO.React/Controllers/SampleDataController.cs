using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;


namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A data controller.
    /// </summary>
    //[Authorize]
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly IJiraClient jiraClient;
        private readonly IAppSettings appSettings;
        private readonly IMemoryCache cache;
        private readonly IBoardCreator boardCreator;
        private readonly IOptions<Paths> paths;

        public SampleDataController(
            IMemoryCache cache, 
            IJiraClient jiraClient, 
            IAppSettings appSettings, 
            IBoardCreator boardCreator, 
            IOptions<Paths> paths
            )
        {
            this.jiraClient = jiraClient;
            this.appSettings = appSettings;
            this.cache = cache;
            this.boardCreator = boardCreator;
            this.paths = paths;
        }

        /// <summary>
        /// Obtains the full currently available board list from Jira REST API.
        /// </summary>
        /// <param name="credentials">Jira credentials for obtaining the data.</param>
        /// <returns>A task of obtaining the list of all currently available Kanban boards.</returns>
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IEnumerable<Value>> BoardList([FromBody] Credentials credentials)
        {
            var credentialsString = credentials.Username + ":" + credentials.Password;

            var boardList = await jiraClient.GetBoardDataAsync<BoardList>("agile/1.0/board/", credentialsString);
            if (boardList == null)
            {
                return appSettings.GetSavedAppSettings()?.Values;
            }  //

            FullBoardList fullBoardList = new FullBoardList();
            fullBoardList.Values.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await jiraClient.GetBoardDataAsync<BoardList>("agile/1.0/board?startAt=" + boardList.StartAt.ToString(), credentialsString);
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

        [HttpPost("[action]")]
        public async Task<IEnumerable<CardColor>> ColorList(int id, [FromBody] Credentials credentials)
        {
            var credentialsString = credentials.Username + ":" + credentials.Password;

            var colorList = new ColorList();
            colorList = await jiraClient.GetBoardDataAsync<ColorList> ("greenhopper/1.0/cardcolors/" + id + "/strategy/priority", credentialsString, id);

            if (colorList == null)
            {
                return null;
            }

            return colorList.CardColors;
        }

        /// <summary>
        /// Gets board data, checks if the data has changed (compared to cached version), saves to cache if it has.
        /// </summary>
        /// <param name="id">Id of the board whose data will be returned.</param>
        /// <param name="credentials">Jira credentials for obtaining the data.</param>
        /// <returns>Board information.</returns>
        [HttpPost("[action]")]
        public async Task<Board> BoardData(int id, [FromBody] Credentials credentials)
        {
            var credentialsString = credentials.Username + ":" + credentials.Password;
            var b = boardCreator.CreateBoardModel(id, credentialsString, cache);
            Board board = null;
            try
            {
                board = await b;
                if (NeedsRedraw(board))
                {
                    board.HasChanged = true;
                    cache.Set(id, board);
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
            if (!cache.TryGetValue(board.Id, out Board cachedBoard)) { return true; }
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
        /// Reads connection log from the appropriate file into an object.
        /// </summary>
        /// <param name="id">Id of the board whose log entries will be retrieved.</param>
        /// <returns>A list of connection log entries.</returns>
        [Authorize]
        [HttpGet("[action]")]
        public List<JiraConnectionLogEntry> NetworkStatistics(int id)
        {
            var filePath = Path.Combine(paths.Value.LogDirectoryPath, id.ToString() + "_jiraConnectionLog.txt");
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
