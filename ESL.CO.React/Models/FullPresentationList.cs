using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESL.CO.React.Models
{
    public class FullPresentationList
    {
        public List<BoardPresentation> PresentationList { get; set; }

        public FullPresentationList()
        {
            PresentationList = new List<BoardPresentation>();
        }
    }
}
