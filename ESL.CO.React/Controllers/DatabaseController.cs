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
    [Produces("application/json")]
    [Route("api/Database")]
    public class DatabaseController : Controller
    {
        private readonly IDbClient dbClient;

        public DatabaseController(IDbClient dbClient)
        {
            this.dbClient = dbClient;
        }


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

            //var boardList = new FullBoardList();
            //boardList = appSettings.GetSavedAppSettings();

            ////updates the board's statistics
            //foreach (var value in boardList.Values)
            //{
            //    if (value.Id == id)
            //    {
            //        checked { value.TimesShown++; } //checked...
            //        value.LastShown = DateTime.Now;
            //        appSettings.SaveAppSettings(boardList);
            //        return;
            //    }
            //}
            //return;
        }

        /// <summary>
        /// Helper method that updates a statistics entry's times shown counter and last shown date.
        /// </summary>
        /// <param name="entry">Statistics entry object whose properties will be updated.</param>
        private void UpdateStatisticsEntry(StatisticsEntry entry)
        {
            checked { entry.TimesShown++; }  // checked...
            entry.LastShown = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public IEnumerable<StatisticsEntry> StatisticsList()
        {
            var list = dbClient.GetList<StatisticsEntry>();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admins")]
        [HttpPost("[action]")]
        public Queue<JiraConnectionLogEntry> NetworkStatistics([FromBody] string id)
        {
            var entry = dbClient.GetOne<StatisticsEntry>(id);
            return entry.NetworkStats;
        }
    }
}