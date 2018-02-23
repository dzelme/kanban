using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class BoardRow  //
    {
        public List<Issue> IssueRow { get; set; }

        public BoardRow()
        {
            IssueRow = new List<Issue>();
        }
    }
}
