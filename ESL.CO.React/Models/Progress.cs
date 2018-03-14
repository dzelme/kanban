using System;


namespace ESL.CO.React.Models
{
    public class Progress
    {
        public int progress { get; set; }
        public int Total { get; set; }
        public int Percent { get; set; }

        public Progress()
        {
            progress = 0;
            Total = 0;
            Percent = 0;
        }
    }
}
