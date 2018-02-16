﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Board
    {
        public int Id { get; set; }
        public List<BoardColumn> Columns { get; set; }
        public List<BoardRow> Rows { get; set; }  

        public Board(int id = 0)
        {
            Id = id;
            Columns = new List<BoardColumn>();
            Rows = new List<BoardRow>();
        }
    }
}
