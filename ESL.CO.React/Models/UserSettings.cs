using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class UserSettings
    {
        public string Id { get; set; }  // username
        public FullBoardList BoardSettingsList { get; set; }

        public UserSettings()
        {
            Id = "";
            BoardSettingsList = new FullBoardList();
        }
    }
}
