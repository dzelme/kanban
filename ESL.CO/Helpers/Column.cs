using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Helpers
{
    public class Column
    {
        private string name;
        private List<Issue> issues;

        public string Name { get => name; set => name = value; }
        public List<Issue> Issues { get => issues; set => issues = value; }
        
        public Column(string name)
        {
            this.name = name;
            issues = new List<Issue>();
        }
    }
}
