using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Board
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public ColumnList BoardColumns { get; set; }
        public int MaxIssueCount { get; set; }

    }
}
