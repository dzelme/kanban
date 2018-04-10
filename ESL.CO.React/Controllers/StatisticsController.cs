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
        /// Saves board view statistics to database.
        /// </summary>
        /// <param name="id">The id of the board whose statistics will be saved.</param>
        /// <returns>A response with status code 200.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SaveBoardViewStatistics([FromBody] string id)
        {
            await dbClient.SaveStatisticsAsync(new StatisticsDbModel(id, "view"));
            return Ok();
        }

        /// <summary>
        /// Gets the statistics data of all boards that have been viewed atleast once.
        /// </summary>
        /// <returns>A list of boards and their statistics data.</returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<StatisticsModel>> GetStatisticsList()
        {
            var statisticsList = await dbClient.GetStatisticsListAsync();
            return statisticsList;
        }

        /// <summary>
        /// Gets the entire collection of network statistics entries.
        /// </summary>
        /// <param name="id">The id of the board whose network statistics will be obtained.</param>
        /// <returns>A collection of network statistics entries.</returns>
        [Authorize(Roles = "Admins")]
        [HttpPost("[action]")]
        public async Task<List<StatisticsConnectionsModel>> GetNetworkStatisticsList([FromBody] string id)
        {
            var connectionStatsList = await dbClient.GetStatisticsConnectionsListAsync(id);
            return connectionStatsList;
        }
    }
}