﻿using System;

namespace ESL.CO.React.Models
{
    public class StatisticsBoardModel
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public int TimesShown { get; set; }
        public DateTime LastShown { get; set; }
    }
}