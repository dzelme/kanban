using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ESL.CO.React.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board

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

    public class BoardList
    {
        public int MaxResults { get; set; }
        public int StartAt { get; set; }
        public bool IsLast { get; set; }
        public List<Value> Values { get; set; }

        /*
        public BoardList()
        {
            MaxResults = 50;
            StartAt = 0;
            IsLast = false;
            Values = new List<Value>();
        }
        */
    }

    public class FullBoardList //: BoardList
    {
        public List<Value> AllValues { get; set; }

        public FullBoardList()
        {
            AllValues = new List<Value>();
        }
    }
}