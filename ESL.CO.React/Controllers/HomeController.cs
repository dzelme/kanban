using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Gets the index page
        /// </summary>
        public IActionResult Index()
        { 
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
