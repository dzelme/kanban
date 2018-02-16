using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class IndexViewModel
    {
        public int[] ColumnLength { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public List<BoardColumn> Columns { get; set; }
    }
}
