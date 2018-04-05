using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class TimeTracking
    {
        public int OriginalEstimateSeconds { get; set; }  
        public int RemainingEstimateSeconds { get; set; } 
        public int TimeSpentSeconds { get; set; }


        public TimeTracking()
        {
            OriginalEstimateSeconds = 0;
            RemainingEstimateSeconds = 0;
            TimeSpentSeconds = 0;
        }
    }
}
