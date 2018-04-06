using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.DbConnection;
using ESL.CO.React.JiraIntegration;
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
        private readonly IJiraClient jiraClient;

        public StatisticsController(IDbClient dbClient, IJiraClient jiraClient)
        {
            this.dbClient = dbClient;
            this.jiraClient = jiraClient;
        }

        /// <summary>
        /// Saves relevant statistics about board views.
        /// </summary>
        /// <param name="id">The id of the board whose statistics will be saved.</param>
        /// <param name="name">The name of the board whose statistics will be saved.</param>
        [HttpPost("[action]")]
        public void SaveToStatistics(string id, [FromBody] string name)
        {
            //// inefficient, finds entry once here, once in update => make update return bool
            //var entry = dbClient.GetOne<StatisticsEntry>(id);
            //if (entry == null)
            //{
            //    entry = new StatisticsEntry(id, name);
            //    UpdateStatisticsEntry(entry);
            //    //dbClient.Save(entry);
            //}
            //else
            //{
            //    UpdateStatisticsEntry(entry);
            //    //dbClient.Update(id, entry);
            //}

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
        public async Task<IEnumerable<StatisticsModel>> GetStatisticsList() //([FromBody] Credentials credentials)
        {
            //var credentialsString = credentials.Username + ":" + credentials.Password;
            //var boardList = await jiraClient.GetBoardDataAsync<BoardList>("board/", credentialsString);
            var statisticsList = await dbClient.GetStatisticsListAsync();

            //foreach (var board in boardList.Values)
            //{
            //    foreach (var statisticsEntry in statisticsList)
            //    {
            //        if(statisticsEntry.BoardId == board.Id.ToString())
            //        {
            //            statisticsEntry.BoardName = board.Name;
            //            break;
            //        }
            //    }
            //}

            return statisticsList;
            //var list = dbClient.GetList<StatisticsEntry>();
            //return list;
        }

        /// <summary>
        /// Gets the entire collection of network statistics entries.
        /// </summary>
        /// <param name="id">The id of the board whose network statistics will be obtained.</param>
        /// <returns>A collection of network statistics entries.</returns>
        [Authorize(Roles = "Admins")]
        [HttpPost("[action]")]
        public async Task<List<JiraConnectionLogEntry>> GetNetworkStatisticsList([FromBody] string id)
        {
            var connectionStatsList = await dbClient.GetStatisticsConnectionsListAsync(id);
            return connectionStatsList;
        }
    }
}