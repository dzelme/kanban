using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Column
    {
        public string Name { get; set; }
        public List<ColumnStatus> Statuses { get; set; }
        public string Max { get; set; }
    }
}
