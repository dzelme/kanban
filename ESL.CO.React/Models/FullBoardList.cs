using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class FullBoardList
    {
        public List<Value> AllValues { get; set; }

        public FullBoardList()
        {
            AllValues = new List<Value>();
        }
    }
}
