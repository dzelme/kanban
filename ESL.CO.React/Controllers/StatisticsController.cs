using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    [Route("api/Database")]
    public class StatisticsController : Controller
    {
        private readonly IDbClient dbClient;

        public StatisticsController(IDbClient dbClient)
        {
            this.dbClient = dbClient;
        }

        /// <summary>
        /// Saves relevant statistics about board views.
        /// </summary>
        /// <param name="id">The id of the board whose statistics will be saved.</param>
        /// <param name="name">The name of the board whose statistics will be saved.</param>
        [HttpPost("[action]")]
        public void SaveToStatistics(string id, [FromBody] string name)
        {
            // inefficient, finds entry once here, once in update => make update return bool
            var entry = dbClient.GetOne<StatisticsEntry>(id);
            if (entry == null)
            {
                entry = new StatisticsEntry(id, name);
                UpdateStatisticsEntry(entry);
                dbClient.Save(entry);
            }
            else
            {
                UpdateStatisticsEntry(entry);
                dbClient.Update(id, entry);
            }

            return;
        }

        /// <summary>
        /// Updates a statistics entry's "times shown" counter and "last shown" date.
        /// </summary>
        /// <param name="entry">Statistics entry object whose properties will be updated.</param>
        private void UpdateStatisticsEntry(StatisticsEntry entry)
        {
            checked { entry.TimesShown++; }  // checked...
            entry.LastShown = DateTime.Now;
        }

        /// <summary>
        /// Gets the statistics data of all boards that have been viewed atleast once.
        /// </summary>
        /// <returns>A list of boards and their statistics data.</returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public IEnumerable<StatisticsEntry> GetStatisticsList()
        {
            var list = dbClient.GetList<StatisticsEntry>();
            return list;
        }

        /// <summary>
        /// Gets the entire collection of network statistics entries.
        /// </summary>
        /// <param name="id">The id of the board whose network statistics will be obtained.</param>
        /// <returns>A collection of network statistics entries.</returns>
        [Authorize(Roles = "Admins")]
        [HttpPost("[action]")]
        public Queue<JiraConnectionLogEntry> GetNetworkStatisticsList([FromBody] string id)
        {
            var entry = dbClient.GetOne<StatisticsEntry>(id);
            return entry.NetworkStats;
        }
    }
}