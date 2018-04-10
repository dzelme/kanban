using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.Models
{
    public class BoardDbModel  //extends BoardProperties.cs
    {
        public string Id { get; set; }

        public bool Visibility { get; set; }  // vai attēlot attiecīgo Paneli slaidrādē;
        [Range(1, 10_000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]  // range values should equal the ones in UserSettings in appsettings.json
        public int RefreshRate { get; set; }  // Paneļa pārzīmēšanas laiks sekundēs, pēc kura beigām tiek attēlots tas pats Panelis.
        [Range(1, 10_000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]  // range values should equal the ones in UserSettings in appsettings.json
        public int TimeShown { get; set; }  // Paneļa attēlošanas laiks sekundēs, pēc kura beigām tiek attēlots nākošais Panelis;

        public BoardDbModel()
        {
            Id = "0";
            Visibility = false;
        }
    }


}