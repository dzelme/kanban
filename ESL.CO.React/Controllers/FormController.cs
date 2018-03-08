using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ESL.CO.React.Models;
using ESL.CO.React.JiraIntegration;

namespace ESL.CO.React.Controllers
{
    [Route("api/[controller]")]
    public class FormController : Controller
    {
        private readonly IAppSettings appSettings;

        public FormController(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }


        [HttpPost] //, ValidateAntiForgeryToken]
        public void SaveSettings([FromBody]Value[] input)  //change name
        {
            if (ModelState.IsValid)
            {
                var boardList = new FullBoardList
                {
                    AllValues = input.ToList()
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