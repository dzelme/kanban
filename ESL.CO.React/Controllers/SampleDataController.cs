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

using ESL.CO.React.DbConnection;

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
        //private readonly IAppSettings appSettings;  //delete when done with db
        private readonly IMemoryCache cache;
        private readonly IBoardCreator boardCreator;
        private readonly IDbClient dbClient;
        private readonly IOptions<Paths> paths;

        public SampleDataController(
            IMemoryCache cache, 
            IJiraClient jiraClient, 
            //IAppSettings appSettings,  //delete when done with db
            IBoardCreator boardCreator,
            IDbClient dbClient,
            IOptions<Paths> paths
            )
        {
            this.jiraClient = jiraClient;
            //this.appSettings = appSettings;  //delete when done with db
            this.cache = cache;
            this.boardCreator = boardCreator;
            this.dbClient = dbClient;
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

            var boardList = await jiraClient.GetBoardDataAsync<BoardList>("board/", credentialsString);
            if (boardList == null)
            {
                return dbClient.GetOne<UserSettings>(credentials.Username)?.BoardSettingsList?.Values;
            }  //

            FullBoardList fullBoardList = new FullBoardList();
            fullBoardList.Values.AddRange(boardList.Values);
            while (!boardList.IsLast)
            {
                boardList.StartAt += boardList.MaxResults;
                boardList = await jiraClient.GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString(), credentialsString);
                if (boardList == null)
                {
                    fullBoardList = AppSettings.MergeSettings(dbClient.GetOne<UserSettings>(credentials.Username)?.BoardSettingsList, fullBoardList);
                    dbClient.Update(credentials.Username, new UserSettings
                    {
                        Id = credentials.Username,
                        BoardSettingsList = fullBoardList
                    });
                    return fullBoardList.Values;
                }
                fullBoardList.Values.AddRange(boardList.Values);
            }

            fullBoardList = AppSettings.MergeSettings(dbClient.GetOne<UserSettings>(credentials.Username)?.BoardSettingsList, fullBoardList);
            dbClient.Update(credentials.Username, new UserSettings
            {
                Id = credentials.Username,
                BoardSettingsList = fullBoardList
            });
            return fullBoardList.Values;

            //var credentialsString = credentials.Username + ":" + credentials.Password;

            //var boardList = await jiraClient.GetBoardDataAsync<BoardList>("board/", credentialsString);
            //if (boardList == null)
            //{
            //    return appSettings.GetSavedAppSettings()?.Values;
            //}  //

            //FullBoardList fullBoardList = new FullBoardList();
            //fullBoardList.Values.AddRange(boardList.Values);
            //while (!boardList.IsLast)
            //{
            //    boardList.StartAt += boardList.MaxResults;
            //    boardList = await jiraClient.GetBoardDataAsync<BoardList>("board?startAt=" + boardList.StartAt.ToString(), credentialsString);
            //    if (boardList == null)
            //    {
            //        fullBoardList = AppSettings.MergeSettings(appSettings.GetSavedAppSettings(), fullBoardList);
            //        appSettings.SaveAppSettings(fullBoardList);
            //        return fullBoardList.Values;
            //    }
            //    fullBoardList.Values.AddRange(boardList.Values);
            //}

            //fullBoardList = AppSettings.MergeSettings(appSettings.GetSavedAppSettings(), fullBoardList);
            //appSettings.SaveAppSettings(fullBoardList);
            //return fullBoardList.Values;
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
    }
}
