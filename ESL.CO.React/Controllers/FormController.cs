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
        [HttpPost] //, ValidateAntiForgeryToken]
        public void SaveSettings([FromBody]Value[] input)  //change name
        {
            if (ModelState.IsValid)
            {
                var boardList = new FullBoardList
                {
                    AllValues = input.ToList()
                };

                var a = new AppSettings();
                a.SaveAppSettings(boardList);
            }
            else
            {
                //alert about invalid data
            }
            
            return;
        }
    }
}