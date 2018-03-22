using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class LdapSettings
    {
        public string LdapServerUrl { get; set; }
        public string DomainPrefix { get; set; }
        public string DefaultUsername { get; set; }
        public string DefaultPassword { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }
        public string AdminCn { get; set; }
    }
}
