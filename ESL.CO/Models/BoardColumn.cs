using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class BoardColumn
    {
        public string Name { get; set; }
        public List<Issue> Issues { get; set; }

        public BoardColumn(string name)
        {
            Name = name;
            Issues = new List<Issue>();
        }
    }
}
