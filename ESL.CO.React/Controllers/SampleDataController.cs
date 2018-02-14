using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ESL.CO.JiraIntegration;

namespace ESL.CO.React.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        /*
        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
        */
        
        ///*
        [HttpGet("[action]")]
        public async Task<IEnumerable<Models.Value>> WeatherForecasts()
        {
            var client = new JiraClient();
            var boardList = await client.GetBoardListAsync("board/");

            var asd = "";
            return boardList.Values;
        }
        //*/

        /*
        [HttpGet("[action]")]
        public Value WeatherForecasts()
        {
            var rng = new Value
            {
                Id = 620,
                Name = "KP",
                Type = "kanban"
            };
            return rng;
        }
        */
        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }

        /*
        //DRY, use the one from the Models folder
        public class Value
        {
            public int Id { get; set; }
            //public string Self { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }
        */
    }
}
