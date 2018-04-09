using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

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
        private readonly IMemoryCache cache;
        private readonly IBoardCreator boardCreator;

        public SampleDataController(
            IMemoryCache cache, 
            IJiraClient jiraClient, 
            IBoardCreator boardCreator
            )
        {
            this.jiraClient = jiraClient;
            this.cache = cache;
            this.boardCreator = boardCreator;
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
        /// Obtains the full currently available board list from Jira REST API.
        /// </summary>
        /// <param name="credentials">Jira credentials for obtaining the data.</param>
        /// <returns>A task of obtaining the list of all currently available Kanban boards.</returns>
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IEnumerable<Value>> BoardList([FromBody] Credentials credentials)
        {
            return await jiraClient.GetFullBoardList(credentials);
        }
        
        /// <summary>
        /// Gets board data, checks if the data has changed (compared to cached version), saves to cache if it has.
        /// </summary>
        /// <param name="id">The id of the board whose data will be returned.</param>
        /// <param name="credentials">Jira credentials for obtaining the data.</param>
        /// <returns>Board information.</returns>
        [HttpPost("[action]")]
        public async Task<Board> BoardData(string id, [FromBody] Credentials credentials)
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
        /// Obtains a list of priority colors for Kanban issues/tickets.
        /// </summary>
        /// <param name="id">Id of the board whose priority colors will be obtained.</param>
        /// <param name="credentials">Jira user login credentials for making requests.</param>
        /// <returns>A list of objects containing data about the board's issue priority colors.</returns>
        [HttpPost("[action]")]
        public async Task<IEnumerable<CardColor>> ColorList(string id, [FromBody] Credentials credentials)
        {
            var credentialsString = credentials.Username + ":" + credentials.Password;
            var colorList = new ColorList();
            colorList = await jiraClient.GetBoardDataAsync<ColorList> ("greenhopper/1.0/cardcolors/" + id + "/strategy/priority", credentialsString, id);

            if (colorList == null) { return null; }
            return colorList.CardColors;
        }
    }
}
