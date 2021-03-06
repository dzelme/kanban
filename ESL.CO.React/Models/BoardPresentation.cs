﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ESL.CO.React.Models
{
    public class BoardPresentation
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Owner { get; set; }
        public Credentials Credentials { get; set; }
        public FullBoardList Boards { get; set; }

        public BoardPresentation()
        {
            Id = "";
            Title = "";
            Owner = "";
            Credentials = null;
            Boards = null;
        }
    }
}
