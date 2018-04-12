using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.DbConnection;
using ESL.CO.React.Models;
using Microsoft.AspNetCore.Authorization;

namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A controller for actions related to application use statistics.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly IDbClient dbClient;

        public StatisticsController(IDbClient dbClient)
        {
            this.dbClient = dbClient;
        }

            /// <summary>
            /// Saves board or presentation view statistics to database.
            /// </summary>
            /// <param name="id">The id of the board or presentation whose statistics will be saved.</param>
            /// <param name="type">The type of view statistics to be saved: board or presentation.</param>
            /// <returns>A response with status code 200.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SaveViewStatistics([FromBody] StatisticsDbModel entry)
        {
            entry.Time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            await dbClient.SaveStatisticsAsync(entry);
            return Ok();
        }


        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<StatisticsPresentationModel>> GetStatisticsPresentationList()
        {
            var statisticsList = await dbClient.GetStatisticsPresentationListAsync();
            return statisticsList;
        }

            /// <summary>
            /// Gets the statistics data of all boards that have been viewed atleast once.
            /// </summary>
            /// <returns>A list of boards and their statistics data.</returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<StatisticsBoardModel>> GetStatisticsBoardList(string boardId)
        {
            var statisticsList = await dbClient.GetStatisticsBoardListAsync(boardId);
            return statisticsList;
        }

            /// <summary>
            /// Gets the entire collection of network statistics entries.
            /// </summary>
            /// <param name="id">The id of the board whose network statistics will be obtained.</param>
            /// <returns>A collection of network statistics entries.</returns>
        [Authorize(Roles = "Admins")]
        [HttpPost("[action]")]
        public async Task<List<StatisticsConnectionModel>> GetStatisticsConnectionList(string boardId, [FromBody] string presentationId)
        {
            var connectionStatsList = await dbClient.GetStatisticsConnectionsListAsync(presentationId, boardId);
            return connectionStatsList;
        }
    }
}