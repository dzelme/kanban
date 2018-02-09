using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class BoardList
    {
        public string IsLast { get; set; }
        public List<Board> AllBoards { get; set; }
    }
}
