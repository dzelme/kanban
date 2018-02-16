using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class Priority
    {
        public string Name { get; set; }
        public string Self { get; set; }
        public string Id { get; set; }

        public Priority()
        {
            Name = string.Empty;
            Id = string.Empty;
        }
    }
}
