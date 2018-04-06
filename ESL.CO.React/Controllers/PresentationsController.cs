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
        public async Task<IActionResult> GetPresentations()
        {
            var boardPresentationDbModelList = await dbClient.GetPresentationsListAsync();

            var boardPresentationList = new List<BoardPresentation>();

            foreach (var boardPresentationDbModel in boardPresentationDbModelList)
            {
                var boardPresentation = new BoardPresentation
                {
                    Id = boardPresentationDbModel.Id,
                    Title = boardPresentationDbModel.Title,
                    Owner = boardPresentationDbModel.Owner,
                    Credentials = boardPresentationDbModel.Credentials,
                    Boards = new FullBoardList()
                };

                foreach (var boardDbModel in boardPresentationDbModel.Boards)
                {
                    var credentialsString = boardPresentationDbModel.Credentials.Username + ":" + boardPresentationDbModel.Credentials.Password;
                    boardPresentation.Boards.Values.Add(new Value
                    {
                        Id = boardDbModel.Id,
                        Name = (await jiraClient.GetBoardDataAsync<BoardName>("board/" + boardDbModel.Id, credentialsString)).Name,
                        Visibility = boardDbModel.Visibility,
                        RefreshRate = boardDbModel.RefreshRate,
                        TimeShown = boardDbModel.TimeShown
                    });
                }

                boardPresentationList.Add(boardPresentation);
            }

                    //foreach (var boardPresentationDbModel in boardPresentationDbModelList)
                    //{
                    //    var boardList = await jiraClient.GetFullBoardList(boardPresentationDbModel.Credentials);
                    //    foreach (var boardDbModel in boardPresentationDbModel.Boards)
                    //    {
                    //        foreach (var )
                    //    }
                    //}


           return Ok(boardPresentationList);

            //var presentationList = dbClient.GetList<BoardPresentation>();
            //return Ok(presentationList);
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
        public async Task<IActionResult> GetAPresentation(string id)
        {
            var boardPresentationDbModel = dbClient.GetOne<BoardPresentationDbModel>(id);
            if (boardPresentationDbModel == null)
            {
                return BadRequest("Presentation with the specified ID not found!");
            }
            else
            {
                // get name from jira
                // alternatively: GET /rest/agile/1.0/board/{boardId} 
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
                var boardList = await jiraClient.GetFullBoardList(boardPresentationDbModel.Credentials);

                foreach (var boardDbModel in boardPresentationDbModel.Boards)
                {
                    foreach (var value in boardList)
                    {
                        if (boardDbModel.Id == value.Id)
                        {
                            boardPresentation.Boards.Values.Add(new Value
                            {
                                Id = boardDbModel.Id,
                                Name = value.Name,
                                Visibility = boardDbModel.Visibility,
                                TimeShown = boardDbModel.TimeShown,
                                RefreshRate = boardDbModel.RefreshRate
                            });
                        }
                    }
                }

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
                    boardPresentation.Id = dbClient.GeneratePresentationId().ToString();  //
                    //dbClient.SavePresentationsAsync(boardPresentation); //
                    
                    //boardPresentation.Id = dbClient.GeneratePresentationId().ToString();
                    //dbClient.Save(boardPresentation);
                }
                else
                {
                    //dbClient.Update(boardPresentation.Id, boardPresentation);
                }
                dbClient.SavePresentationsAsync(boardPresentation); //
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
            dbClient.Remove<BoardPresentationDbModel>(id);
        }
    }
}