using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ESL.CO.React.Models
{
    public class Value : BoardDbModel
    {
        public string Name { get; set; }

        public Value()
        {
            Name = "";
        }
    }
}