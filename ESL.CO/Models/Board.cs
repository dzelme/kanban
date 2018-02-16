using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.Models
{
    public class Board
    {
        public int Id { get; set; }
        public List<BoardColumn> Columns { get; set; }

        public Board(int id)
        {
            Id = id;
            Columns = new List<BoardColumn>();
        }
    }

}
