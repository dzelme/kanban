using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class ColumnList
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public string BoardType { get; set; }
        public List<Column> AllColumns { get; set; }
    }
}
