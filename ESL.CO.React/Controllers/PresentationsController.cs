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
    [Produces("application/json")]
    [Route("api/admin/[controller]")]
    public class PresentationsController : Controller
    {
        private readonly IJiraClient jiraClient;
        private readonly IAppSettings appSettings;
        private readonly IDbClient dbClient;

        public PresentationsController(
            IJiraClient jiraClient,
            IAppSettings appSettings,
            IDbClient dbClient
            )
        {
            this.jiraClient = jiraClient;
            this.appSettings = appSettings;
            this.dbClient = dbClient;
        }

        [Authorize(Roles = "Admins")]
        [HttpGet]
        public IActionResult GetPresentations()
        {
            //GET / api / presentations - atgriež sarakstu ar prezentācijām(sākotnēji bez paging un limitiem, par to domās vēlāk ja būs nepieciešams)
            //Atbilde 401 Unauthorized, gadījumos ja lietotājs nav autorizēts
            //Atbilde 200 ok un json BoardPresentation(bez credentials sadaļas)

            var presentationList = dbClient.GetList<BoardPresentation>();
            return Ok(presentationList);

            //var presentationList = appSettings.GetPresentationList();
            //return Ok(presentationList.PresentationList);
        }

        [HttpGet("{id}")]
        public IActionResult GetAPresentation(string id)
        {
            //GET / api / presentations /{ id} -atgriež pieprasīto prezentāciju
            //Atbilde 404 Not Found, ja prezentācija nav atrasta
            //Atbilde 200 OK un json BoardPresentation(bez credentials sadaļas)

            var boardPresentation = dbClient.GetOne<BoardPresentation>(id);
            //var boardPresentation = appSettings.GetPresentation(id);
            if (boardPresentation == null)
            {
                return BadRequest("Presentation with the specified ID not found!");
            }
            else
            {
                return Ok(boardPresentation);
            }
        }

        [Authorize(Roles = "Admins")]
        [HttpPost]
        public IActionResult SavePresentation([FromBody] BoardPresentation boardPresentation)
        {
            //POST / api / presentations - izveido jaunu vai saglabā esošu prezentāciju
            //Atbilde 401 Unauthorized, gadījumos ja lietotājs nav autorizēts
            //Atbilde 404 Not Found, ja norādīts id, bet sistēmā tāds nav atrodams
            //Atbilde 200 OK un saglabātā informācija(bez credentials sadaļas)

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
                //appSettings.SavePresentation(boardPresentation);
            }
            else
            {
                return BadRequest("invalid data"); //
            }

            boardPresentation.Credentials = null;  // better way?
            return Ok (boardPresentation);
        }

        [Authorize(Roles = "Admins")]
        [HttpDelete("{id}")]
        public void DeletePresentation(string id)
        {
            dbClient.Remove<BoardPresentation>(id);
            //appSettings.DeletePresentation(id);
        }
    }
}