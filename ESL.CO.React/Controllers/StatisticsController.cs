using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.DbConnection;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
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
        /// Saves board or presentation view statistics to database.
        /// </summary>
        /// <param name="entry">A statistics database entry model containing board id, presentation id and save request type.</param>
        /// <returns>A response with status code 200.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SaveViewStatistics([FromBody] StatisticsDbModel entry)
        {
            entry.Time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            await dbClient.SaveStatisticsAsync(entry);
            return Ok();
        }

        /// <summary>
        /// Gets view statistics with names and id lists for all presentations that have been viewed at least once.
        /// </summary>
        /// <returns>A list of objects containing appended presentation view statistics.</returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<StatisticsPresentationModel>> GetStatisticsPresentationList()
        {
            var statisticsList = await dbClient.GetStatisticsPresentationListAsync();
            statisticsList = await AddTitleAndBoardIds(statisticsList);
            return statisticsList;
        }

        /// <summary>
        /// Gets presentation specific view statistics with board names for all boards that have been viewed as part of the specified presentation at least once.
        /// </summary>
        /// <param name="presentationId">The id of the presentation whose board view statistics will be obtained.</param>
        /// <returns>A list of objects containing appended board view statistics.</returns>
        [Authorize(Roles = "Admins")]
        [HttpGet("[action]")]
        public async Task<IEnumerable<StatisticsBoardModel>> GetStatisticsBoardList(string presentationId)
        {
            var statisticsList = await dbClient.GetStatisticsBoardListAsync(presentationId);
            statisticsList = await AddNames(statisticsList, presentationId);
            return statisticsList;
        }

        /// <summary>
        /// Gets presentation specific Jira request statistics for a specified board as part of the specified presentation.
        /// </summary>
        /// <param name="boardId">The id of the board whose connection statistics will be obtained.</param>
        /// <param name="presentationId">The id of the presentation in whose context connection statistics for the specified board will be obtained.</param>
        /// <returns>A list of objects containing information about requests to Jira REST API for the specified board.</returns>
        [Authorize(Roles = "Admins")]
        [HttpPost("[action]")]
        public async Task<List<StatisticsConnectionModel>> GetStatisticsConnectionList(string boardId, [FromBody] string presentationId)
        {
            var connectionStatsList = await dbClient.GetStatisticsConnectionsListAsync(presentationId, boardId);
            return connectionStatsList;
        }

        /// <summary>
        /// Adds board names to the specified presentation's list of board database representation models.
        /// </summary>
        /// <param name="statisticsList">The presentation's board database representation model list whose elements will be appended.</param>
        /// <param name="presentationId">The id of the presentation whose board database representation model list will be appended.</param>
        /// <returns>The appended board database representation model list.</returns>
        private async Task<IEnumerable<StatisticsBoardModel>> AddNames(IEnumerable<StatisticsBoardModel> statisticsList, string presentationId)
        {
            var credentials = (await dbClient.GetPresentation(presentationId))?.Credentials;
            var boardList = await jiraClient.GetBoardDataAsync<BoardList>("agile/1.0/board/", credentials);
            foreach (var boardModel in statisticsList)
            {
                foreach (var board in boardList.Values)
                {
                    if (boardModel.BoardId == board.Id) { boardModel.BoardName = board.Name; }
                }
            }
            return statisticsList;
        }

        /// <summary>
        /// Adds a title and board id list to each presentation model in the given presentation model list.
        /// </summary>
        /// <param name="statisticsList">A list of presentation database representation models each of which will be appended.</param>
        /// <returns>The list of appended presentation models.</returns>
        private async Task<IEnumerable<StatisticsPresentationModel>> AddTitleAndBoardIds(IEnumerable<StatisticsPresentationModel> statisticsList)
        {
            var appendedStatisticsList = new List<StatisticsPresentationModel>();
            foreach (var presentationModel in statisticsList)
            {
                var presentationDbEntry = await dbClient.GetPresentation(presentationModel.PresentationId);
                presentationModel.Title = presentationDbEntry.Title;
                presentationModel.Boards = new FullBoardList { Values = new List<Value>() };
                foreach (var board in presentationDbEntry.Boards)
                {
                    presentationModel.Boards.Values.Add(new Value { Id = board.Id });
                }
                appendedStatisticsList.Add(presentationModel);
            }
            return appendedStatisticsList;
        }
    }
}