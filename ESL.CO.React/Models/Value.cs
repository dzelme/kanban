using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Value
    {
        public int Id { get; set; }
        //public string Self { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public bool Visibility { get; set; }  // vai attēlot attiecīgo Paneli slaidrādē;

        [Range(1000, 100_000)]
        public int RefreshRate { get; set; }  // Paneļa pārzīmēšanas laiks sekundēs, pēc kura beigām tiek attēlots tas pats Panelis.

        [Range(1000, 100_000)]
        public int TimeShown { get; set; }  // Paneļa attēlošanas laiks sekundēs, pēc kura beigām tiek attēlots nākošais Panelis;


        public Value()
        {
            Id = 0;
            Name = "";
            Type = "";
            Visibility = false;
            RefreshRate = 10_000;
            TimeShown = 1000;
        }
    }


}