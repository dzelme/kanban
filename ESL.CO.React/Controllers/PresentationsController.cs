using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.DbConnection;
using ESL.CO.React.LdapCredentialCheck;
using Microsoft.AspNetCore.Authorization;

namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A controller for actions related to presentations.
    /// </summary>
    [Produces("application/json")]
    [Route("api/admin/[controller]")]
    public class PresentationsController : Controller
    {
        private readonly IJiraClient jiraClient;
        private readonly IDbClient dbClient;
        private readonly ILdapClient ldapClient;

        public PresentationsController(
            IJiraClient jiraClient,
            IDbClient dbClient,
            ILdapClient ldapClient
        )
        {
            this.jiraClient = jiraClient;
            this.dbClient = dbClient;
            this.ldapClient = ldapClient;
        }

        /// <summary>
        /// Helper function to get board names from Jira.
        /// </summary>
        /// <param name="boardPresentationDbModel">An object containing presentation data stored in db, which will be supplemented with board names from Jira.</param>
        /// <returns>An object containing all necessary information about a presentation.</returns>
        private async Task<BoardPresentation> AddNameToPresentationBoards(BoardPresentationDbModel boardPresentationDbModel)
        {
            var boardPresentation = new BoardPresentation
            {
                Id = boardPresentationDbModel.Id,
                Title = boardPresentationDbModel.Title,
                Owner = boardPresentationDbModel.Owner,
                Credentials = boardPresentationDbModel.Credentials,
                Boards = new FullBoardList
                {
                    Values = new List<Value>()
                }
            };

            foreach (var boardDbModel in boardPresentationDbModel.Boards)
            {
                var boardName = await jiraClient.GetBoardDataAsync<BoardName>("agile/1.0/board/" + boardDbModel.Id, boardPresentation.Credentials);
                boardPresentation.Boards.Values.Add(new Value
                {
                    Id = boardDbModel.Id,
                    Name = (boardName == null) ? "<panelis nepieejams>" : boardName.Name,
                    Visibility = boardDbModel.Visibility,
                    TimeShown = boardDbModel.TimeShown,
                    RefreshRate = boardDbModel.RefreshRate
                });
            }

            return boardPresentation;
        }

        /// <summary>
        /// Gets a list of all saved presentations.
        /// </summary>
        /// <returns>
        /// A response with status code 200 together with an object containing a list of presentations and their respective data.
        /// </returns>
        [Authorize(Roles = "Admins")]
        [HttpGet]
        public async Task<IActionResult> GetPresentations()
        {
            var boardPresentationDbModelList = await dbClient.GetPresentationsListAsync();
            var boardPresentationList = new List<BoardPresentation>();

            foreach (var boardPresentationDbModel in boardPresentationDbModelList)
            {
                var boardPresentation = await AddNameToPresentationBoards(boardPresentationDbModel);
                boardPresentation.Credentials = null;
                boardPresentationList.Add(boardPresentation);
            }

           return Ok(boardPresentationList);
        }

        /// <summary>
        /// Gets a single presentation.
        /// </summary>
        /// <param name="id">The id of the presentation to be obtained.</param>
        /// <returns>
        /// A response with status code 400 if invalid id was passed and
        /// a response with status code 200 together with an object containing all data about the presentation.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPresentation(string id)
        {
            var boardPresentationDbModel = await dbClient.GetPresentation(id);
            if (boardPresentationDbModel == null)
            {
                return BadRequest("Presentation with the specified ID not found!");
            }
            else
            {
                var boardPresentation = await AddNameToPresentationBoards(boardPresentationDbModel);
                boardPresentation.Credentials.Password = null;
                return Ok(boardPresentation);
            }
        }

        /// <summary>
        /// Saves the passed presentation in database.
        /// </summary>
        /// <param name="boardPresentation">An object containing all data about the presentation.</param>
        /// <returns>
        /// A response with status code 400 if invalid data was passed,
        /// a response with status code 401 if user's credentials are not authorized,
        /// a response with status code 200 together with the saved presentation data excluding credentials if save is successful.
        /// </returns>
        [Authorize(Roles = "Admins")]
        [HttpPost]
        public async Task<IActionResult> SavePresentation([FromBody] BoardPresentation boardPresentation)
        {
            if(!string.IsNullOrEmpty(boardPresentation.Id) && boardPresentation.Credentials.Password == null)
            {
                boardPresentation.Credentials = (await dbClient.GetPresentation(boardPresentation.Id)).Credentials;
            }

            if (ldapClient.CheckCredentials(boardPresentation.Credentials.Username, boardPresentation.Credentials.Password, false))
            {
                if (ModelState.IsValid)
                {
                    await dbClient.SavePresentationsAsync(boardPresentation);
                }
                else
                {
                    return BadRequest("Invalid user input. Check allowed range for time values.");
                }

                boardPresentation.Credentials = null;
                return Ok(boardPresentation);
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Deletes the specified presentation from the database.
        /// </summary>
        /// <param name="id">The id of the presentation to be deleted.</param>
        [Authorize(Roles = "Admins")]
        [HttpDelete("{id}")]
        public void DeletePresentation(string id)
        {
            dbClient.DeletePresentation(id);
        }
    }
}