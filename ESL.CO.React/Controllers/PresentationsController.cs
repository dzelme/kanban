using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using ESL.CO.React.DbConnection;
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

        public PresentationsController(
            IJiraClient jiraClient,
            IDbClient dbClient
            )
        {
            this.jiraClient = jiraClient;
            this.dbClient = dbClient;
        }

        /// <summary>
        /// Gets a list of all saved presentations.
        /// </summary>
        /// <returns>
        /// A response with status code 200 together with an object containing a list of presentations and their respective data.
        /// </returns>
        [Authorize(Roles = "Admins")]
        [HttpGet]
        public IActionResult GetPresentations()
        {
            var presentationList = dbClient.GetList<BoardPresentation>();
            return Ok(presentationList);
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
        public IActionResult GetPresentation(string id)
        {
            var boardPresentation = dbClient.GetOne<BoardPresentation>(id);
            if (boardPresentation == null)
            {
                return BadRequest("Presentation with the specified ID not found!");
            }
            else
            {
                return Ok(boardPresentation);
            }
        }

        /// <summary>
        /// Saves the passed presentation in database.
        /// </summary>
        /// <param name="boardPresentation">An object containing all data about the presentation.</param>
        /// <returns>
        /// A response with status code 400 if invalid data was passed and
        /// a response with status code 200 together with the saved presentation data excluding credentials.
        /// </returns>
        [Authorize(Roles = "Admins")]
        [HttpPost]
        public IActionResult SavePresentation([FromBody] BoardPresentation boardPresentation)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(boardPresentation.Id))
                {
                    boardPresentation.Id = dbClient.GeneratePresentationId().ToString();
                    dbClient.Save(boardPresentation);
                }
                else
                {
                    dbClient.Update(boardPresentation.Id, boardPresentation);
                }
            }
            else
            {
                return BadRequest("invalid data"); //
            }

            boardPresentation.Credentials = null;  // better way?
            return Ok (boardPresentation);
        }

        /// <summary>
        /// Deletes the specified presentation from the database.
        /// </summary>
        /// <param name="id">The id of the presentation to be deleted.</param>
        [Authorize(Roles = "Admins")]
        [HttpDelete("{id}")]
        public void DeletePresentation(string id)
        {
            dbClient.Remove<BoardPresentation>(id);
        }
    }
}