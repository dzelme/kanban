using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Assignee
    {
        //public string self { get; set; }
        //public string name { get; set; }  //username
        //public string key { get; set; }
        //public string emailAddress { get; set; }
        //public AvatarUrls2 avatarUrls { get; set; }  //avatar image in 4 sizes (48x48, ..., 16x16)
        public string DisplayName { get; set; }  //full name
        //public bool active { get; set; }
        //public string timeZone { get; set; }

        public Assignee()
        {
            DisplayName = string.Empty;
        }
    }
}
