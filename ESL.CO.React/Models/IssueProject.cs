using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class IssueProject
    {
        public string Id { get; set; }  //e.g., 15786
        public string Key { get; set; }  //e.g., KP
        public string Name { get; set; }  //e.g., KOSMOSS Prakse

        public IssueProject()
        {
            Id = string.Empty;
            Key = string.Empty;
            Name = string.Empty;
        }
    }
}
