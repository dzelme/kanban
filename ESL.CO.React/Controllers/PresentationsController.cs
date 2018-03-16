using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;
using Microsoft.AspNetCore.Authorization;

namespace ESL.CO.React.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PresentationsController : Controller
    {
        private readonly IJiraClient jiraClient;
        private readonly IAppSettings appSettings;

        public PresentationsController(IJiraClient jiraClient, IAppSettings appSettings)
        {
            this.jiraClient = jiraClient;
            this.appSettings = appSettings;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Presentation(int id)
        {
            //GET / api / presentations /{ id} -atgriež pieprasīto prezentāciju
            //Atbilde 404 Not Found, ja prezentācija nav atrasta
            //Atbilde 200 OK un json BoardPresentation(bez credentials sadaļas)

            var boardPresentation = appSettings.GetPresentation(id);
            if (boardPresentation == null)
            {
                return BadRequest("Presentation with the specified ID not found!");
            }
            else
            {
                return Ok(boardPresentation);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult PresentationList()
        {
            //GET / api / presentations - atgriež sarakstu ar prezentācijām(sākotnēji bez paging un limitiem, par to domās vēlāk ja būs nepieciešams)
            //Atbilde 401 Unauthorized, gadījumos ja lietotājs nav autorizēts
            //Atbilde 200 ok un json BoardPresentation(bez credentials sadaļas)

            var presentationList = appSettings.GetPresentationList();
            return Ok(presentationList);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SavePresentation([FromBody] BoardPresentation boardPresentation)
        {
            //POST / api / presentations - izveido jaunu vai saglabā esošu prezentāciju
            //Atbilde 401 Unauthorized, gadījumos ja lietotājs nav autorizēts
            //Atbilde 404 Not Found, ja norādīts id, bet sistēmā tāds nav atrodams
            //Atbilde 200 OK un saglabātā informācija(bez credentials sadaļas)

            if (ModelState.IsValid)
            {
                boardPresentation.Credentials = null;  // better way?
                appSettings.SavePresentation(boardPresentation);
            }
            else
            {
                //alert about invalid data
                return BadRequest("invalid data"); //
            }

            return Ok (boardPresentation);
        }
    }
}