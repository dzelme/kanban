using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Controllers
{
    [Route("/api/version")]
    [Produces("application/json")]
    public class VersionController : Controller
    {
        [HttpGet]
        public string GetVersion()
        {
            return typeof(SampleDataController).Assembly.GetName().Version.ToString();
        }
    }
}
