using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;

namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A controller for working with forms.
    /// </summary>
    [Route("api/[controller]")]
    public class FormController : Controller
    {
        private readonly IAppSettings appSettings;

        public FormController(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        /// <summary>
        /// Saves the altered application settings.
        /// </summary>
        /// <param name="input">New application settings received from form data via a post request.</param>
        [HttpPost] //, ValidateAntiForgeryToken]
        public void SaveSettings([FromBody]Value[] input)  //change name
        {
            if (ModelState.IsValid)
            {
                var boardList = new FullBoardList
                {
                    Values = input.ToList()
                };

                appSettings.SaveAppSettings(boardList);
            }
            else
            {
                //alert about invalid data
            }
            
            return ;
        }
    }
}