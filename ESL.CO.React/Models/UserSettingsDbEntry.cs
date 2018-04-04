using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class UserSettingsDbEntry
    {
        public string Id { get; set; }  // username
        public FullBoardList BoardSettingsList { get; set; }

        public UserSettingsDbEntry()
        {
            Id = "";
            BoardSettingsList = new FullBoardList();
        }
    }
}
