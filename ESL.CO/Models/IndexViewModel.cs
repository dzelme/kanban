using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    //model that is passed to view from controller
    public class IndexViewModel
    {
        public int[] ColumnLength { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
        public List<BoardColumn> Columns { get; set; }
    }
}
