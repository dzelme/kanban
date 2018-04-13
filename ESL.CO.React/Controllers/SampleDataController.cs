using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.Models;
using ESL.CO.React.DbConnection;
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
        private readonly IDbClient dbClient;

        public SampleDataController(
            IMemoryCache cache, 
            IJiraClient jiraClient, 
            IBoardCreator boardCreator,
            IDbClient dbClient
            )
        {
            this.jiraClient = jiraClient;
            this.cache = cache;
            this.boardCreator = boardCreator;
            this.dbClient = dbClient;
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
            return (await jiraClient.GetFullBoardList(credentials)).OrderBy(board => Convert.ToInt32(board.Id));
        }
        
        /// <summary>
        /// Gets board data, checks if the data has changed (compared to cached version), saves to cache if it has.
        /// </summary>
        /// <param name="id">The id of the board whose data will be returned.</param>
        /// <param name="credentials">Jira credentials for obtaining the data.</param>
        /// <returns>Board information.</returns>
        [HttpPost("[action]")]
        public async Task<Board> BoardData(string boardId, [FromBody] string presentationId)
        {
            var credentials = (await dbClient.GetPresentation(presentationId))?.Credentials;
            var credentialsString = credentials.Username + ":" + credentials.Password;
            var b = boardCreator.CreateBoardModel(boardId, presentationId, credentialsString, cache);
            Board board = null;
            try
            {
                board = await b;
                if (NeedsRedraw(board))
                {
                    board.HasChanged = true;
                    cache.Set(boardId, board);
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
    }
}
