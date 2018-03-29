using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ESL.CO.React.Models;
using ESL.CO.React.DbConnection;
using Microsoft.AspNetCore.Authorization;

namespace ESL.CO.React.Controllers
{
    /// <summary>
    /// A controller for saving user settings.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IDbClient dbClient;

        public SettingsController(IDbClient dbClient)
        {
            this.dbClient = dbClient;
        }

        [HttpPost("[action]")] //, ValidateAntiForgeryToken]
        public void SaveUserSettings(string id, [FromBody] Value[] input)  //change name
        {
            if (ModelState.IsValid)
            {
                var boardList = new FullBoardList
                {
                    Values = input.ToList()
                };

                // optional: when creating a new presentation no need to remember previously selected boards
                // is useful to remember "time shown" and "refresh rate" values
                foreach (Value value in boardList.Values)
                {
                    value.Visibility = false;
                }

                dbClient.Update(id, new UserSettings
                {
                    Id = id,
                    BoardSettingsList = boardList
                });
            }
            else
            {
                //alert about invalid data
            }

            return;
        }
    }
}