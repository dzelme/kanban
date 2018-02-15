using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Column
    {
        public string Name { get; set; }
        public List<ColumnStatus> Statuses { get; set; }  //which issues shown in this column
        //public int? Max { get; set; }  //max number of issues in one column
    }
}
