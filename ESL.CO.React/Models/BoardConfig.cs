using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;

namespace ESL.CO.React.Models
{
    //obtained from https://jira.returnonintelligence.com/rest/agile/1.0/board/963/configuration

    public class BoardConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ColumnConfig ColumnConfig { get; set; }
    }
}
