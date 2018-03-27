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

        /// <summary>
        /// Increases the statistics counter each time the board is shown.
        /// </summary>
        /// <param name="id">Id of the board whose counter will be increased.</param>
        [HttpPost("[action]")]
        public void IncrementTimesShown([FromBody] string id, [FromBody] string name)
        {
            var entry = dbClient.GetStatisticsEntry(id);
            if (entry == null)
            {
                entry = new StatisticsEntry(id, name);
            }

            checked { entry.TimesShown++; }  // checked...
            entry.LastShown = DateTime.Now;
            dbClient.SaveStatisticsEntry(entry);

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
        /// Reads connection log from the appropriate file into an object.
        /// </summary>
        /// <param name="id">Id of the board whose log entries will be retrieved.</param>
        /// <returns>A list of connection log entries.</returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public JiraConnectionLogEntry[] NetworkStatistics(string id)
        {
            var entry = dbClient.GetStatisticsEntry(id);
            return entry.NetworkStats;

            //var filePath = Path.Combine(paths.Value.LogDirectoryPath, id.ToString() + "_jiraConnectionLog.txt");
            //var connectionLog = new List<JiraConnectionLogEntry>();
            //if (System.IO.File.Exists(filePath))
            //{
            //    using (StreamReader r = new StreamReader(filePath))
            //    {
            //        var logEntry = r.ReadLine();
            //        while (logEntry != null)
            //        {
            //            string[] logEntryField = logEntry.Split('|');
            //            connectionLog.Add(new JiraConnectionLogEntry(
            //                logEntryField[1],
            //                logEntryField[2],
            //                (logEntryField.Length > 3) ? logEntryField[3] : "",
            //                logEntryField[0]
            //            ));
            //            logEntry = r.ReadLine();
            //        }
            //    }
            //}
            //return connectionLog;
        }

        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public IEnumerable<StatisticsEntry> StatisticsList()
        {
            var list = dbClient.GetStatisticsList();
            return list;
        }
    }
}