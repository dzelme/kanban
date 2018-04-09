using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.Models
{
    public class Value  //extends BoardProperties.cs
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool Visibility { get; set; }  // vai attēlot attiecīgo Paneli slaidrādē;
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]  // range values should equal the ones in UserSettings in appsettings.json
        public int RefreshRate { get; set; }  // Paneļa pārzīmēšanas laiks sekundēs, pēc kura beigām tiek attēlots tas pats Panelis.
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]  // range values should equal the ones in UserSettings in appsettings.json
        public int TimeShown { get; set; }  // Paneļa attēlošanas laiks sekundēs, pēc kura beigām tiek attēlots nākošais Panelis;

        public Value()
        {
            Id = "0";
            Name = "";
            Visibility = false;
        }
    }


}